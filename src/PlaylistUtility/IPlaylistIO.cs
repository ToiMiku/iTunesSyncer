
using System;
using System.IO;

namespace iTunesSyncer.PlaylistUtility
{
    public interface IPlaylistIO
    {
        public string ImportFilePattern { get; }
        public string ExportFileExtension { get; }

        public Playlist Load(Stream stream);

        public void Save(Stream stream, Playlist playlist);
    }

    public class PlaylistIOUtillity
    {
        public static IPlaylistIO CreateIPlaylistIO(FormatSelect formatSelect)
        {
            switch (formatSelect)
            {
            case FormatSelect.M3U:
                return new M3UPlaylistIO();

            case FormatSelect.M3U8:
                return new M3U8PlaylistIO();

            case FormatSelect.XSPF:
                return new XMLPlaylistIO();

            default:
                throw new NotImplementedException();
            }
        }
    }
}
