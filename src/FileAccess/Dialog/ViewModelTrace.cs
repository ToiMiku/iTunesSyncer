using Reactive.Bindings;
using System.Windows;

namespace iTunesSyncer.FileAccess.Dialog
{
    public partial class ViewModel
    {
        private readonly Window _window;
        private readonly IFileSystem _fileSystem;

        public ViewModel(Window window, IFileSystem fileSystem)
        {
            _window = window;
            _fileSystem = fileSystem;

            InitCommands();

            InitProperties();
        }

        private void InitProperties()
        {
        }

        private void InitCommands()
        {
            FormLoadedCommand = new ReactiveCommand()
                .WithSubscribe(FormLoadedCommandAction);

            MoveParentDirectoryCommand = new ReactiveCommand(IsEnabledParentDirectory)
                .WithSubscribe(MoveParentDirectoryCommandAction);

            UpdateCurrentDirectoryCommand = new ReactiveCommand(IsEnabledForAccess)
                .WithSubscribe(UpdateCurrentDirectoryCommandAction);

            CreateDirectoryCommand = new ReactiveCommand(IsEnabledForAccess)
                .WithSubscribe(CreateDirectoryCommandAction);

            DeleteItemCommand = new ReactiveCommand(IsEnabledSelectedItem)
                .WithSubscribe(DeleteItemCommandAction);

            ListItemSelectionChangedCommand = new ReactiveCommand()
                .WithSubscribe(ListItemSelectionChangedCommandAction);

            ListItemMouseDoubleClickCommand = new ReactiveCommand()
                .WithSubscribe(ListItemMouseDoubleClickCommandAction);

            OkCommand = new ReactiveCommand()
                .WithSubscribe(OkCommandAction);

            CancelCommand = new ReactiveCommand()
                .WithSubscribe(CancelCommandAction);
        }
    }
}
