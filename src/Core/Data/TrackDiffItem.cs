using iTunesSyncer.DataDiffListUtility;
using iTunesSyncer.PlaylistUtility;

namespace iTunesSyncer.Core.Data
{
    public class TrackDiffItem : DataDiffListItem<Track>
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

        public TrackDiffItem(Track item1, Track item2) : base(item1, item2)
        {
            Status = DiffStatus.Checking;
        }

        public TrackDiffItem(DataDiffListItem<Track> origin) : base(origin)
        {
        }
    }
}
