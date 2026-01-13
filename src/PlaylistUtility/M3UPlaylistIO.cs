using System.IO;
using System.Linq;

namespace iTunesSyncer.PlaylistUtility
{
    class M3UPlaylistIO : IPlaylistIO
    {
        public string ImportFilePattern => "*.m3u";

        public string ExportFileExtension => ".m3u";

        public M3UPlaylistIO()
        {
        }

        public Playlist Load(Stream stream)
        {
            var playlist = new Playlist();

            using var reader = new StreamReader(stream);

            string? filename = null;
            while ((filename = reader.ReadLine()) != null)
            {
                // コメントは除外
                if (filename.StartsWith('#'))
                    continue;

                AddSong(playlist, filename);
            }

            return playlist;
        }

        private void AddSong(Playlist playlist, string filename)
        {
            var track = new Track
            {
                Location = filename,
                Title = Path.GetFileName(filename)
            };
            playlist.Tracks.Add(track);
        }

        public void Save(Stream stream, Playlist playlist)
        {
            var writer = new StreamWriter(stream);

            foreach (var filename in playlist.Tracks.Select(x => x.Title))
            {
                writer.WriteLine(filename);
            }

            writer.Flush();
        }
    }
}
