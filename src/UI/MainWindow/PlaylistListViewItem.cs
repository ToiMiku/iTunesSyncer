using iTunesSyncer.Core.Data;
using Reactive.Bindings;
using System.Windows.Media;

namespace iTunesSyncer.UI.MainWindow
{
    public class PlaylistListViewItem
    {
        private readonly Color NormalColor = (Color)ColorConverter.ConvertFromString("Transparent");
        private readonly Color ModifyColor = (Color)ColorConverter.ConvertFromString("PeachPuff");
        private readonly Color ErrorColor = (Color)ColorConverter.ConvertFromString("Red");

        public ReactiveProperty<string> StatusText { get; } = new();
        public ReactiveProperty<Brush> Background { get; } = new();
        public ReactiveProperty<string> PlaylistTitle { get; } = new();

        public PlaylistDiffItem PlaylistDiffItem { get; private set; }

        public PlaylistListViewItem(PlaylistDiffItem playlistDiffItem)
        {
            PlaylistDiffItem = playlistDiffItem;

            PlaylistTitle.Value = playlistDiffItem.NotNullItem.Title;

            UpdateStausText();
        }

        private void UpdateStausText()
        {
            switch (PlaylistDiffItem.Status)
            {
            case PlaylistDiffItem.DiffStatus.Checking:
                StatusText.Value = "チェック中...";
                Background.Value = new SolidColorBrush(NormalColor);
                break;
            case PlaylistDiffItem.DiffStatus.Equal:
                StatusText.Value = "差分なし";
                Background.Value = new SolidColorBrush(NormalColor);
                break;
            case PlaylistDiffItem.DiffStatus.Added:
                StatusText.Value = "新規追加";
                Background.Value = new SolidColorBrush(ModifyColor);
                break;
            case PlaylistDiffItem.DiffStatus.Deleted:
                StatusText.Value = "削除";
                Background.Value = new SolidColorBrush(ModifyColor);
                break;
            case PlaylistDiffItem.DiffStatus.Updated:
                StatusText.Value = "更新";
                Background.Value = new SolidColorBrush(ModifyColor);
                break;
            case PlaylistDiffItem.DiffStatus.Synchronizing:
                StatusText.Value = "同期中...";
                Background.Value = new SolidColorBrush(NormalColor);
                break;
            case PlaylistDiffItem.DiffStatus.Synchronized:
                StatusText.Value = "同期完了";
                Background.Value = new SolidColorBrush(NormalColor);
                break;
            case PlaylistDiffItem.DiffStatus.Error:
                StatusText.Value = "エラー";
                Background.Value = new SolidColorBrush(ErrorColor);
                break;
            }
        }
    }
}
