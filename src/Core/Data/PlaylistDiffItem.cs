using iTunesSyncer.DataDiffListUtility;
using iTunesSyncer.PlaylistUtility;
using System.Collections.Generic;

namespace iTunesSyncer.Core.Data
{
    public class PlaylistDiffItem : DataDiffListItem<Playlist>
    {
        public enum DiffStatus
        {
            Checking,

            Equal,
            Added,
            Deleted,
            Updated,

            Synchronizing,
            Synchronized,

            Error
        }

        public DiffStatus Status { get; set; }

        public IList<TrackDiffItem> TrackDiffItems { get; set; }

        public PlaylistDiffItem(Playlist item1, Playlist item2) : base(item1, item2)
        {
            Status = DiffStatus.Checking;
            TrackDiffItems = new List<TrackDiffItem>();
        }

        public PlaylistDiffItem(DataDiffListItem<Playlist> origin) : base(origin)
        {
        }
    }
}
