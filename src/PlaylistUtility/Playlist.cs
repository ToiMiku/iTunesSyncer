using System.Collections.Generic;
using System.IO;

namespace iTunesSyncer.PlaylistUtility
{
    public class Playlist
    {
        public string Title { get; set; }

        public List<Track> Tracks { get; set; }

        public string Location { get; set; }

        public Playlist()
        {
            Title = "";
            Tracks = new List<Track>();
        }

        public Playlist(Playlist originPlaylist)
        {
            Title = originPlaylist.Title;
            Location = originPlaylist.Location;
            Tracks = new List<Track>(originPlaylist.Tracks.Count);

            foreach (var originTrack in originPlaylist.Tracks)
            {
                Tracks.Add(new Track(originTrack));
            }
        }

        public Playlist(Stream stream, IPlaylistIO playlistIO)
        {
            var playlist = Load(stream, playlistIO);

            Title = playlist.Title;
            Tracks = playlist.Tracks;
            Location = playlist.Location;
        }

        private Playlist Load(Stream stream, IPlaylistIO playlistIO)
        {
            var playlist = playlistIO.Load(stream);

            return playlist;
        }

        public void Save(Stream stream, IPlaylistIO playlistIO)
        {
            playlistIO.Save(stream, this);
        }
    }
}
