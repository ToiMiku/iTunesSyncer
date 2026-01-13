using iTunesSyncer.Core.Data;
using iTunesSyncer.DataDiffListUtility;
using iTunesSyncer.Diagnostics;
using iTunesSyncer.Extensions;
using iTunesSyncer.FileAccess;
using iTunesSyncer.iTunesExport;
using iTunesSyncer.PlaylistUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace iTunesSyncer.Core
{
    public class ImportUtility
    {
        public static IList<Playlist> LoadPlaylistsFromITunes(string iTunesLibraryPath)
        {
            // iTunesLibraryからプレイリストを作成
            var iTunesExport = new ITunesExport(iTunesLibraryPath);
            var playlists = new List<Playlist>(iTunesExport.Export());

            return playlists;
        }

        public static void UpdateRelativePathInPlaylists(IList<Playlist> playlists, string referencePath)
        {
            foreach (var playlist in playlists)
            {
                foreach (var track in playlist.Tracks)
                {
                    //パスがおかしいTrackがあったらエラーにする
                    if (!PathExtension.IsPathInDirectory(referencePath, track.Location))
                        throw new Exception("対応できないパス：" + track.Location);

                    // 相対パスに変換してTrackOptionに保存
                    var relativePath = Path.GetRelativePath(referencePath, track.Location);
                    relativePath = relativePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    relativePath = PathExtension.UriEncode(relativePath);
                    track.SetRelativePath(relativePath);
                }
            }
        }

        public static IList<Playlist> LoadExportPlaylists(string exportPath, IFileSystem exportFileSystem, IPlaylistIO playlistIO)
        {
            var playlists = new List<Playlist>();

            foreach (var filename in exportFileSystem.DirectoryAccess.EnumerateFiles(exportPath, playlistIO.ImportFilePattern))
            {
                using var stream = FileControl.OpenRead(filename, exportFileSystem);

                Playlist playlist;
                try
                {
                    playlist = new Playlist(stream, playlistIO);
                }
                catch (Exception e)
                {
                    // エラーログは出しておく
                    Logger.GetInstance().Error(e);
                    continue;
                }

                playlists.Add(playlist);
            }

            return playlists;
        }

        public static IList<PlaylistDiffItem> GeneratePlaylistDiffList
            (IList<Playlist> importPlaylists, IList<Playlist> exportPlaylists)
        {
            // プレイリストの差分リストを作成
            var diffList = DataDiffListGenerator<Playlist>
                .GenerateWithoutOrder(importPlaylists, exportPlaylists, (x, y) =>
                {
                    return string.Compare(x.Title, y.Title);
                });

            var resultList = new List<PlaylistDiffItem>(diffList.Count);
            foreach (var diffItem in diffList)
            {
                resultList.Add(new PlaylistDiffItem(diffItem));
            }

            return resultList;
        }

        public static List<TrackDiffItem> GenerateTrackDiffList
            (IList<Track> importTracks, IList<Track> exportTracks)
        {
            List<DataDiffListItem<Track>> diffList;

            if (importTracks is not null && exportTracks is not null)
            {
                diffList = DataDiffListGenerator<Track>.Generate(importTracks, exportTracks, (inp, exp) =>
                {
                    return string.Compare(inp.GetRelativePath(), exp.GetRelativePath());
                });
            }
            else if (importTracks is not null)
            {
                diffList = new List<DataDiffListItem<Track>>(importTracks.Count);
                foreach (var track in importTracks)
                {
                    diffList.Add(new DataDiffListItem<Track>(track, null));
                }
            }
            else// if (exportTracks is not null)
            {
                diffList = new List<DataDiffListItem<Track>>(exportTracks.Count);
                foreach (var track in exportTracks)
                {
                    diffList.Add(new DataDiffListItem<Track>(null, track));
                }
            }

            var resultList = new List<TrackDiffItem>(diffList.Count);
            foreach (var diffItem in diffList)
            {
                resultList.Add(new TrackDiffItem(diffItem));
            }

            return resultList;
        }

        public static void UpdateTrackDiffItemStatus(TrackDiffItem trackDiffItem, FileHashDictionary fileHashDictionary)
        {
            var track1 = trackDiffItem.Item1;
            var track2 = trackDiffItem.Item2;
            var hash1 = string.Empty;
            var hash2 = string.Empty;

            // 同期元のファイルのハッシュを計算してトラックに格納
            if (track1 is not null)
            {
                if (File.Exists(track1.Location))
                {
                    hash1 = fileHashDictionary.GetHash(track1.Location);
                    track1.SetFileHash(hash1);
                }

                if (hash1 == string.Empty)
                {
                    trackDiffItem.Status = TrackDiffItem.DiffStatus.Error;
                    // エラーログは出しておく
                    Logger.GetInstance().Error(
                        string.Format("Unable to compute the hash of the file: {0}", track1.Location));
                    return;
                }
            }

            if (track2 is not null)
            {
                hash2 = track2?.GetFileHash();
            }
            

            // 差分をチェック
            if (track1 is not null && track2 is not null)
            {
                trackDiffItem.Status = (hash1 == hash2)
                    ? TrackDiffItem.DiffStatus.Equal : TrackDiffItem.DiffStatus.Updated;
                return;
            }

            if (track1 is not null)
            {
                trackDiffItem.Status = TrackDiffItem.DiffStatus.Added;
                return;
            }

            if (track2 is not null)
            {
                trackDiffItem.Status = TrackDiffItem.DiffStatus.Deleted;
                return;
            }

            throw new Exception("来るはずないとこ来た");
        }

        public static void UpdatePlaylistDiffItemStatus(PlaylistDiffItem playlistDiffItem)
        {
            var counts = GetTrackDiffItemStatusCount(playlistDiffItem.TrackDiffItems);
            var isAnyAdded = counts.nAdded > 0;
            var isAnyDeleted = counts.nDeleted > 0;
            var isAnyUpdated = counts.nUpdated > 0;
            var isAnyError = counts.nError > 0;

            if (isAnyError)
            {
                playlistDiffItem.Status = PlaylistDiffItem.DiffStatus.Error;
                return;
            }

            if (isAnyUpdated || (isAnyAdded && isAnyDeleted))
            {
                playlistDiffItem.Status = PlaylistDiffItem.DiffStatus.Updated;
                return;
            }

            if (isAnyAdded)
            {
                playlistDiffItem.Status = PlaylistDiffItem.DiffStatus.Added;
                return;
            }

            if (isAnyDeleted)
            {
                playlistDiffItem.Status = PlaylistDiffItem.DiffStatus.Deleted;
                return;
            }

            playlistDiffItem.Status = PlaylistDiffItem.DiffStatus.Equal;
        }

        public static (int nEqual, int nAdded, int nDeleted, int nUpdated, int nError) GetTrackDiffItemStatusCount
            (IEnumerable<TrackDiffItem> trackDiffItems)
        {
            var nEqual = 0;
            var nAdded = 0;
            var nDeleted = 0;
            var nUpdated = 0;
            var nError = 0;

            if (trackDiffItems is not null)
            {
                foreach (var trackStatus in trackDiffItems.Select(x => x.Status))
                {
                    switch (trackStatus)
                    {
                    case TrackDiffItem.DiffStatus.Equal: nEqual++; break;
                    case TrackDiffItem.DiffStatus.Added: nAdded++; break;
                    case TrackDiffItem.DiffStatus.Deleted: nDeleted++; break;
                    case TrackDiffItem.DiffStatus.Updated: nUpdated++; break;
                    case TrackDiffItem.DiffStatus.Error: nError++; break;

                    default:
                        throw new Exception("ありえないの来た");
                    }
                }
            }

            return (nEqual, nAdded, nDeleted, nUpdated, nError);
        }
    }
}
