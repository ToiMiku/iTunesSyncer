using iTunesSyncer.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace iTunesSyncer.iTunesExport
{
    public class ITunesExport
    {
        private const string RemoveLocationHeader = @"file://localhost/";

        private readonly string[] ExclusionPlaylistNames = new string[]
        {
            "ライブラリ",
            "ダウンロード済み",
            "ムービー",
            "テレビ番組",
            "ポッドキャスト",
            "オーディオブック",
            "Genius",
            /*
            "ミュージック",
            "00_VOCALOID",
            "01_とーいのプレイリスト",
            "02_ともみのプレイリスト",
            "03_SnowMiku",*/
        };

        private ITunesLibraryParser.ITunesLibrary _library;

        public ITunesExport(string iTunesLibraryPath)
        {
            _library = new ITunesLibraryParser.ITunesLibrary(iTunesLibraryPath);
        }

        public IEnumerable<PlaylistUtility.Playlist> Export()
        {
            foreach (var iTunesPlaylist in _library.Playlists)
            {
                // 楽曲のプレイリスト以外は除外
                if (ExclusionPlaylistNames.Any(x => x == iTunesPlaylist.Name))
                    continue;

                yield return ConvertPlaylist(iTunesPlaylist);
            }
        }

        private PlaylistUtility.Playlist ConvertPlaylist(ITunesLibraryParser.Playlist iTunesPlaylist)
        {
            var playlist = new PlaylistUtility.Playlist()
            {
                Title = iTunesPlaylist.Name,
                Location = String.Empty,
            };

            foreach (var iTunesTrack in iTunesPlaylist.Tracks)
            {
                var track = ConvertTrack(iTunesTrack);
                playlist.Tracks.Add(track);
            }

            return playlist;
        }

        private PlaylistUtility.Track ConvertTrack(ITunesLibraryParser.Track iTunesTrack)
        {
            // URLデコードして保存場所を取得
            var location = iTunesTrack.Location;
            location = location.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            location = PathExtension.UriDecode(location);

            // 先頭の文字列を除去
            if (location.StartsWith(RemoveLocationHeader))
            {
                location = location.Substring(RemoveLocationHeader.Length);
            }

            var track = new PlaylistUtility.Track()
            {
                Title = iTunesTrack.Name,
                Location = location,
            };

            return track;
        }
    }
}
