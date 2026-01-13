using Reactive.Bindings;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace iTunesSyncer.UI.MainWindow
{
    public partial class MainWindowViewModel
    {
        // InputGroup
        public ReactiveProperty<bool> IsEnabledInputGroup { get; } = new(true);
        public ReactiveProperty<string> ImportPath { get; } = new();
        public ICommand OpenImportPathCommand { get; private set; }
        public ReactiveProperty<string> ReferencePath { get; } = new();
        public ICommand OpenReferencePathCommand { get; private set; }
        public ReactiveProperty<int> ExportSelectedIndex { get; } = new();
        public ICommand ExportSelectedChangedCommand { get; private set; }
        public ReactiveProperty<string> ReadOnlyExportPath { get; } = new();
        public ICommand OpenExportSettingCommand { get; private set; }



        // Import
        public ReactiveProperty<bool> IsEnabledmportButton { get; } = new(true);
        public ReactiveProperty<string> ImportButtonText { get; } = new();
        public ICommand ImportPlaylistCommand { get; private set; }



        // ListView
        public ObservableCollection<PlaylistListViewItem> PlaylistListViewSource { get; } = new();
        public ReactiveProperty<PlaylistListViewItem> PlaylistSelectedItem { get; } = new();
        public ReactiveProperty<int> PlaylistSelectedIndex { get; } = new();
        public ObservableCollection<TrackListViewItem> TrackListViewSource { get; } = new();
        public ICommand PlaylistSelectedChangedCommand { get; private set; }
        public ReactiveProperty<ImageSource> TrackListViewStatusFilterIcon { get; } = new();



        // TrackListView Popup
        public ReactiveProperty<bool> IsOpenTrackStatusPopup { get; } = new(false);
        public ICommand ClickTrackListViewColumnCommand { get; private set; }
        public ReactiveProperty<bool> IsVisibleEqualTrackItem { get; } = new(true);
        public ReactiveProperty<bool> IsVisibleAddedTrackItem { get; } = new(true);
        public ReactiveProperty<bool> IsVisibleDeletedTrackItem { get; } = new(true);
        public ReactiveProperty<bool> IsVisibleUpdatedTrackItem { get; } = new(true);
        public ReactiveProperty<bool> IsVisibleErrorTrackItem { get; } = new(true);
        public ReactiveProperty<string> VisibleEqualTrackItemText { get; } = new("差分なし");
        public ReactiveProperty<string> VisibleAddedTrackItemText { get; } = new("追加");
        public ReactiveProperty<string> VisibleDeletedTrackItemText { get; } = new("削除");
        public ReactiveProperty<string> VisibleUpdatedTrackItemText { get; } = new("差分あり");
        public ReactiveProperty<string> VisibleErrorTrackItemText { get; } = new("エラー");
        public ICommand TrackStatusPopupOkCommand { get; private set; }




        // Syncronize
        public ReactiveProperty<bool> IsEnabledSyncronizeButton { get; } = new(true);
        public ReactiveProperty<string> SyncronizeButtonText { get; } = new();
        public ICommand SyncronizeCommand { get; private set; }



        // StatusBar
        public ReactiveProperty<int> ProgressBarRatio { get; } = new();
        public ReactiveProperty<string> ProgressLabelRatio { get; } = new();
        public ReactiveProperty<string> ProgressText { get; } = new();
        public ReactiveProperty<Brush> ProgressTextBrush { get; } = new();
    }
}
