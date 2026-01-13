using System;
using System.IO;
using System.Windows;


namespace iTunesSyncer.FileAccess.Dialog
{
    /// <summary>
    /// OpenFileSystemDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class OpenFileSystemDialog : Window
    {
        private readonly IFileSystem _fileSystem;

        public string InitialDirectory { get; set; }

        public bool IsDirectoryPicker { get; set; }

        public string FileName { get; private set; }

        public OpenFileSystemDialog(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public new void Show()
        {
            throw new NotSupportedException();
        }

        public new bool? ShowDialog()
        {
            if (!IsDirectoryPicker)
                throw new NotSupportedException();

            var viewModel = new ViewModel(this, _fileSystem)
            {
                IsDirectoryPicker = IsDirectoryPicker
            };
            viewModel.CurrentDirectoryPath.Value = InitialDirectory;
            this.DataContext = viewModel;

            // コンポーネントの初期化
            InitializeComponent();

            var ret = base.ShowDialog();

            FileName = (ret == true)
                    ? Path.Combine(viewModel.CurrentDirectoryPath.Value, viewModel.FileOrDirectoryName.Value)
                    : string.Empty;

            return ret;
        }
    }
}
