using Reactive.Bindings;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace iTunesSyncer.FileAccess.Dialog
{
    public partial class ViewModel
    {
        public bool IsDirectoryPicker { get; set; }


        public ReactiveProperty<bool> IsEnabledForAccess { get; } = new();
        public ReactiveProperty<bool> IsEnabledParentDirectory { get; } = new();
        public ReactiveProperty<bool> IsEnabledSelectedItem { get; } = new(false);
        public ReactiveProperty<string> CurrentDirectoryPath { get; } = new();
        public ReactiveProperty<string> FileOrDirectoryName { get; } = new();
        public ObservableCollection<FileListViewItem> FileListViewItemSource { get; } = new();
        public ReactiveProperty<FileListViewItem> FileListSelectedItem { get; } = new();

        public ICommand FormLoadedCommand { get; private set; }
        public ICommand MoveParentDirectoryCommand { get; private set; }
        public ICommand UpdateCurrentDirectoryCommand { get; private set; }
        public ICommand CreateDirectoryCommand { get; private set; }
        public ICommand DeleteItemCommand { get; private set; }
        public ICommand ListItemSelectionChangedCommand { get; private set; }
        public ICommand ListItemMouseDoubleClickCommand { get; private set; }
        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
    }
}
