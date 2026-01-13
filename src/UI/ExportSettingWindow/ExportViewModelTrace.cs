using iTunesSyncer.Core.Config;
using Reactive.Bindings;
using System.Windows;

namespace iTunesSyncer.UI.ExportSettingWindow
{
    public partial class ExportViewModel
    {
        private readonly Window _window;

        public ExportViewModel(Window window)
        {
            _window = window;

            InitUIValue();
            InitCommands();
        }

        public void InitUIValue()
        {
            ExportSelected.Value = ConfigManager.Instance.Property.Export.Select;

            LocalExportPath.Value = ConfigManager.Instance.Property.Export.Local.Path;

            FtpHost.Value = ConfigManager.Instance.Property.Export.Ftp.Host;
            FtpPort.Value = ConfigManager.Instance.Property.Export.Ftp.Port;
            FtpUserName.Value = ConfigManager.Instance.Property.Export.Ftp.UserName;
            FtpPassword.Value = ConfigManager.Instance.Property.Export.Ftp.GetPassword();
            FtpExportPath.Value = ConfigManager.Instance.Property.Export.Ftp.Path;

            PlaylistFormatSelectedIndex.Value = ConfigManager.Instance.Property.PlaylistFormatSelectIndex;
        }

        private void InitCommands()
        {
            OpenLocalExportPathCommand = new ReactiveCommand().
                WithSubscribe(OpenLocalExportPathCommandAction);

            OpenFtpExportPathCommand = new ReactiveCommand().
                WithSubscribe(OpenFtpExportPathCommandAction);

            ExportSettingOkCommand = new ReactiveCommand().
                WithSubscribe(OkCommandAction);
            ExportSettingCancelCommand = new ReactiveCommand().
                WithSubscribe(CancelCommandAction);

            PlaylistFormatSelectedChangedCommand = new ReactiveCommand()
                .WithSubscribe(PlaylistFormatSelectedChangedCommandAction);
        }
    }
}
