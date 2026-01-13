using iTunesSyncer.FileAccess.Dialog;
using Microsoft.WindowsAPICodePack.Dialogs;
using iTunesSyncer.FtpFileAccess;
using iTunesSyncer.Core.Config;

namespace iTunesSyncer.UI.ExportSettingWindow
{
    public partial class ExportViewModel
    {
        private void OpenLocalExportPathCommandAction()
        {
            using var dialog = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください",
                InitialDirectory = LocalExportPath.Value,
                // フォルダ選択モードにする
                IsFolderPicker = true,
            };
            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                LocalExportPath.Value = dialog.FileName;
            }
        }


        private void OpenFtpExportPathCommandAction()
        {
            var fs = new FtpFileSystem(
                FtpHost.Value, FtpPort.Value, FtpUserName.Value, FtpPassword.Value);

            var dialog = new OpenFileSystemDialog(fs)
            {
                Title = "フォルダを選択してください",
                InitialDirectory = FtpExportPath.Value,
                IsDirectoryPicker = true,
            };
            var result = dialog.ShowDialog();

            if (result == true)
            {
                FtpExportPath.Value = dialog?.FileName;
            }
        }

        public void PlaylistFormatSelectedChangedCommandAction()
        {
            ConfigManager.Instance.Property.PlaylistFormatSelectIndex = PlaylistFormatSelectedIndex.Value;
            ConfigManager.Instance.Save();
        }


        private void OkCommandAction()
        {
            ConfigManager.Instance.Property.Export.Select = ExportSelected.Value;

            ConfigManager.Instance.Property.Export.Local.Path = LocalExportPath.Value;

            ConfigManager.Instance.Property.Export.Ftp.Host = FtpHost.Value;
            ConfigManager.Instance.Property.Export.Ftp.Port = FtpPort.Value;
            ConfigManager.Instance.Property.Export.Ftp.UserName = FtpUserName.Value;
            ConfigManager.Instance.Property.Export.Ftp.SetPassword(FtpPassword.Value);
            ConfigManager.Instance.Property.Export.Ftp.Path = FtpExportPath.Value;

            ConfigManager.Instance.Save();

            _window.DialogResult = true;
            _window.Close();

        }

        private void CancelCommandAction()
        {
            _window.DialogResult = false;
            _window.Close();
        }
    }
}
