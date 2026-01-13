using iTunesSyncer.Core.Data;
using Reactive.Bindings;
using System.Windows.Media;

namespace iTunesSyncer.UI.MainWindow
{
    public class TrackListViewItem
    {
        private readonly Color NormalColor = (Color)ColorConverter.ConvertFromString("Transparent");
        private readonly Color ModifyColor = (Color)ColorConverter.ConvertFromString("PeachPuff");
        private readonly Color ErrorColor = (Color)ColorConverter.ConvertFromString("Red");

        public ReactiveProperty<string> StatusText { get; } = new();
        public ReactiveProperty<Brush> Background { get; } = new();
        public ReactiveProperty<string> SoundTitle { get; } = new();


        public TrackDiffItem TrackDiffItem { get; private set; }

        public TrackListViewItem(TrackDiffItem trackDiffItem)
        {
            TrackDiffItem = trackDiffItem;

            SoundTitle.Value = trackDiffItem.NotNullItem.Title;

            UpdateStausText();
        }

        private void UpdateStausText()
        {
            switch (TrackDiffItem.Status)
            {
            case TrackDiffItem.DiffStatus.Checking:
                StatusText.Value = "チェック中...";
                Background.Value = new SolidColorBrush(NormalColor);
                break;
            case TrackDiffItem.DiffStatus.Equal:
                StatusText.Value = "差分なし";
                Background.Value = new SolidColorBrush(NormalColor);
                break;
            case TrackDiffItem.DiffStatus.Added:
                StatusText.Value = "追加";
                Background.Value = new SolidColorBrush(ModifyColor);
                break;
            case TrackDiffItem.DiffStatus.Deleted:
                StatusText.Value = "削除";
                Background.Value = new SolidColorBrush(ModifyColor);
                break;
            case TrackDiffItem.DiffStatus.Updated:
                StatusText.Value = "更新";
                Background.Value = new SolidColorBrush(ModifyColor);
                break;
            case TrackDiffItem.DiffStatus.Synchronizing:
                StatusText.Value = "同期中...";
                Background.Value = new SolidColorBrush(NormalColor);
                break;
            case TrackDiffItem.DiffStatus.Synchronized:
                StatusText.Value = "同期完了";
                Background.Value = new SolidColorBrush(NormalColor);
                break;
            case TrackDiffItem.DiffStatus.Error:
                StatusText.Value = "エラー";
                Background.Value = new SolidColorBrush(ErrorColor);
                break;
            }
        }
    }
}
