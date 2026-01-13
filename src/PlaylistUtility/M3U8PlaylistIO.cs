using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using iTunesSyncer.Extensions;

namespace iTunesSyncer.PlaylistUtility
{
    class M3U8PlaylistIO : IPlaylistIO
    {
        private readonly string TAG_EXTM3U = "#EXTM3U";

        private readonly string TAG_PLAYLIST_TITLE = "#ITS-X-PLAYLIST-TITLE:";
        private readonly string TAG_PLAYLIST_LOCATION = "#ITS-X-PLAYLIST-LOCATION:";

        private readonly string TAG_TRACK_TITLE = "#ITS-X-TRACK-TITLE:";
        private readonly string TAG_TRACK_FILEHASH = "#ITS-X-TRACK-FILEHASH:";
        private readonly string TAG_TRACK_REACTIVEPATH = "#ITS-X-TRACK-REACTIVEPATH:";

        public string ImportFilePattern => "*.m3u8";

        public string ExportFileExtension => ".m3u8";

        public M3U8PlaylistIO()
        {
        }

        public Playlist Load(Stream stream)
        {
            using var reader = new StreamReader(stream, encoding: System.Text.Encoding.UTF8, leaveOpen: true);

            var playlistInfo = FindPlaylistInformation(reader);

            var playlist = new Playlist()
            {
                Title = playlistInfo.title,
                Location = playlistInfo.location,
            };

            foreach (var playlistEntry in ParsePlaylistEnumerator(reader))
            {
                var track = new Track
                {
                    Title = playlistEntry.Title,
                    Location = playlistEntry.Location,
                    Options = new List<TrackOption>(),
                };

                track.SetRelativePath(playlistEntry.ReactivePath);
                track.SetFileHash(playlistEntry.FileHash);

                playlist.Tracks.Add(track);
            }

            return playlist;
        }

        private (string title, string location) FindPlaylistInformation(StreamReader reader)
        {
            string? currentTitle = null;
            string? currentLocation = null;

            while (!reader.EndOfStream && (currentTitle is null || currentLocation is null))
            {
                var line = reader.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                if (line.StartsWith(TAG_PLAYLIST_TITLE, StringComparison.OrdinalIgnoreCase))
                {
                    currentTitle = line.Substring(TAG_PLAYLIST_TITLE.Length).Trim();
                }
                else if (line.StartsWith(TAG_PLAYLIST_LOCATION, StringComparison.OrdinalIgnoreCase))
                {
                    currentLocation = line.Substring(TAG_PLAYLIST_LOCATION.Length).Trim()
                        .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                }
                else
                {
                    // 謎のタグは無視
                }
            }

            return (currentTitle, currentLocation);
        }

        private class PlaylistEntry
        {
            public string? Title { get; set; }
            public string? FileHash { get; set; }
            public string? ReactivePath { get; set; }
            public string Location { get; set; } = "";
        }

        private IEnumerable<PlaylistEntry> ParsePlaylistEnumerator(StreamReader reader)
        {
            string? currentTitle = null;
            string? currentHash = null;
            string? currentReactivePath = null;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                if (line.StartsWith(TAG_TRACK_TITLE, StringComparison.OrdinalIgnoreCase))
                {
                    currentTitle = line.Substring(TAG_TRACK_TITLE.Length).Trim();
                }
                else if (line.StartsWith(TAG_TRACK_FILEHASH, StringComparison.OrdinalIgnoreCase))
                {
                    currentHash = line.Substring(TAG_TRACK_FILEHASH.Length).Trim();
                }
                else if (line.StartsWith(TAG_TRACK_REACTIVEPATH, StringComparison.OrdinalIgnoreCase))
                {
                    currentReactivePath = line.Substring(TAG_TRACK_REACTIVEPATH.Length).Trim();
                    currentReactivePath = currentReactivePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                }
                else if (!line.StartsWith("#"))
                {
                    var location = line;
                    location = location.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                    yield return new PlaylistEntry
                    {
                        Title = currentTitle,
                        FileHash = currentHash,
                        ReactivePath = currentReactivePath,
                        Location = location
                    };

                    // 状態リセット
                    currentTitle = null;
                    currentHash = null;
                    currentReactivePath = null;
                }
                else
                {
                    // 謎のタグは無視
                }
            }
        }

        public void Save(Stream stream, Playlist playlist)
        {
            using var writer = new StreamWriter(stream, encoding: System.Text.Encoding.UTF8, leaveOpen: true);

            Save(writer, playlist);
        }

        private void Save(StreamWriter writer, Playlist playlist)
        {
            writer.WriteLine(TAG_EXTM3U);
            writer.WriteLine("");

            writer.WriteLine(TAG_PLAYLIST_TITLE + playlist.Title);
            writer.WriteLine(TAG_PLAYLIST_LOCATION + playlist.Location.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
            writer.WriteLine("");

            foreach (var playlistEntry in SummarizePlaylistEnumerator(playlist))
            {
                writer.WriteLine("#EXT-X-RATING:0");
                writer.WriteLine(TAG_TRACK_TITLE + playlistEntry.Title);
                writer.WriteLine(TAG_TRACK_FILEHASH + playlistEntry.FileHash);
                writer.WriteLine(TAG_TRACK_REACTIVEPATH + playlistEntry.ReactivePath);
                writer.WriteLine(playlistEntry.Location.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
                writer.WriteLine("");
            }

            writer.Flush();
        }

        private IEnumerable<PlaylistEntry> SummarizePlaylistEnumerator(Playlist playlist)
        {
            foreach (var track in playlist.Tracks)
            {
                yield return new PlaylistEntry()
                {
                    Title = track.Title,
                    FileHash = track.GetFileHash(),
                    ReactivePath = track.GetRelativePath(),
                    Location = track.Location,
                };
            }
        }
    }
}
