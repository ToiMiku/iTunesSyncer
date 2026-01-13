using iTunesSyncer.Core.Config;
using Reactive.Bindings;
using System.Windows.Input;

namespace iTunesSyncer.UI.ExportSettingWindow
{
    public partial class ExportViewModel
    {
        // RadioButton

        public ReactiveProperty<ExportSelect> ExportSelected { get; } = new();


        // ローカルフォルダ
        public ReactiveProperty<string> LocalExportPath { get; } = new();
        public ICommand OpenLocalExportPathCommand { get; private set; }


        // FTP
        public ReactiveProperty<string> FtpHost { get; } = new();
        public ReactiveProperty<int> FtpPort { get; } = new();
        public ReactiveProperty<string> FtpUserName { get; } = new();
        public ReactiveProperty<string> FtpPassword { get; } = new();
        public ReactiveProperty<string> FtpExportPath { get; } = new();
        public ICommand OpenFtpExportPathCommand { get; private set; }


        // 詳細設定
        public ReactiveProperty<int> PlaylistFormatSelectedIndex { get; } = new();
        public ICommand PlaylistFormatSelectedChangedCommand { get; private set; }


        // OK,Cancel
        public ICommand ExportSettingOkCommand { get; private set; }
        public ICommand ExportSettingCancelCommand { get; private set; }
    }
}
