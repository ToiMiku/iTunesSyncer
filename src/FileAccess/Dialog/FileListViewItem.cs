using iTunesSyncer.Properties.Resources;
using Reactive.Bindings;
using System.Windows.Media;

namespace iTunesSyncer.FileAccess.Dialog
{
    public class FileListViewItem
    {
        public enum FileObjectType
        {
            None,
            File,
            Directory
        }

        public ReactiveProperty<string> Name { get; } = new();
        public ReactiveProperty<string> TypeText { get; } = new();
        public ReactiveProperty<ImageSource> Icon { get; } = new();

        private FileObjectType _type;
        public FileObjectType Type
        {
            get => _type;
            set
            {
                if (_type == value) return;

                _type = value;
                UpdateTypeText();
            }
        }

        public FileListViewItem(string name, FileObjectType type)
        {
            Name.Value = name;
            Type = type;
        }

        private void UpdateTypeText()
        {
            switch(Type)
            {
            case FileObjectType.None:
                TypeText.Value = "";
                Icon.Value = null;
                break;
            case FileObjectType.Directory:
                TypeText.Value = "ディレクトリ";
                Icon.Value = ResourceManager.GetBitmapImage(Resource.Directory);
                break;
            case FileObjectType.File:
                TypeText.Value = "ファイル";
                Icon.Value = ResourceManager.GetBitmapImage(Resource.File);
                break;
            }
        }
    }
}
