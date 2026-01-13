using iTunesSyncer.Core;
using iTunesSyncer.Core.Config;
using iTunesSyncer.Core.Data;
using iTunesSyncer.Core.DataSet;
using iTunesSyncer.Diagnostics;
using iTunesSyncer.FileAccess;
using iTunesSyncer.FtpFileAccess;
using iTunesSyncer.PlaylistUtility;
using iTunesSyncer.Properties.Resources;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;

namespace iTunesSyncer.UI.MainWindow
{
    partial class MainWindowViewModel
    {
        public enum RunningStatus
        {
            NoImport,
            Importing,
            Ready,
            Syncronizing,
            Completed
        }

        private RunningStatus _status;
        public RunningStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                UpdateUI();
            }
        }


        private BackgroundWorker _importExecute = null;
        private FileHashDictionary _fileHashDictionary = new();
        private BackgroundWorker _syncronizeExecute = null;
        private bool _isVisibleEqualTrackItem = true;
        private bool _isVisibleAddedTrackItem = true;
        private bool _isVisibleDeletedTrackItem = true;
        private bool _isVisibleUpdatedTrackItem = true;
        private bool _isVisibleErrorTrackItem = true;

        private void UpdateUI()
        {
            switch (Status)
            {
            case RunningStatus.NoImport:
                IsEnabledInputGroup.Value = true;
                IsEnabledmportButton.Value = true;
                IsEnabledSyncronizeButton.Value = false;
                break;

            case RunningStatus.Importing:
                IsEnabledInputGroup.Value = false;
                IsEnabledmportButton.Value = true;
                IsEnabledSyncronizeButton.Value = false;
                break;

            case RunningStatus.Ready:
                IsEnabledInputGroup.Value = true;
                IsEnabledmportButton.Value = true;
                IsEnabledSyncronizeButton.Value = true;
                break;

            case RunningStatus.Syncronizing:
                IsEnabledInputGroup.Value = false;
                IsEnabledmportButton.Value = false;
                IsEnabledSyncronizeButton.Value = true;
                break;

            case RunningStatus.Completed:
                IsEnabledInputGroup.Value = true;
                IsEnabledmportButton.Value = true;
                IsEnabledSyncronizeButton.Value = false;
                break;
            }

            ImportButtonText.Value = (Status != RunningStatus.Importing) ? "インポート" : "キャンセル";
            SyncronizeButtonText.Value = (Status != RunningStatus.Syncronizing) ? "同期" : "キャンセル";
        }

        public void OpenImportPathCommandAction()
        {
            using var dialog = new CommonOpenFileDialog()
            {
                Title = "インポート元のファイルを選択してください",
                InitialDirectory = ImportPath.Value,
                // ファイル選択モードにする
                IsFolderPicker = false,
            };
            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                ImportPath.Value = dialog.FileName;

                ConfigManager.Instance.Property.Import.LocalPath = dialog.FileName;
                ConfigManager.Instance.Save();
            }
        }

        public void OpenReferencePathCommandAction()
        {
            using var dialog = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください",
                InitialDirectory = ReferencePath.Value,
                // フォルダ選択モードにする
                IsFolderPicker = true,
            };
            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                ReferencePath.Value = dialog.FileName;

                ConfigManager.Instance.Property.Import.ReferencePath = dialog.FileName;
                ConfigManager.Instance.Save();
            }
        }

        public void ExportSelectedChangedCommandAction()
        {
            ConfigManager.Instance.Property.Export.SelectIndex = ExportSelectedIndex.Value;
            UpdateReadOnlyExportPath();
        }

        public void OpenExportSettingCommandAction()
        {
            var exportDialog = new ExportSettingWindow.ExportSettingWindow();
            exportDialog.Owner = Application.Current.MainWindow;
            exportDialog.ShowDialog();

            // キャンセル時はfalseまたはnullなので、trueじゃなければキャンセルとする
            if (exportDialog.DialogResult != true)
                return;

            ExportSelectedIndex.Value = ConfigManager.Instance.Property.Export.SelectIndex;
            UpdateReadOnlyExportPath();
        }

        private void UpdateReadOnlyExportPath()
        {
            switch (ConfigManager.Instance.Property.Export.Select)
            {
            case ExportSelect.Local:
                ReadOnlyExportPath.Value = ConfigManager.Instance.Property.Export.Local.Path; break;
            case ExportSelect.Ftp:
                ReadOnlyExportPath.Value = ConfigManager.Instance.Property.Export.Ftp.Path; break;
            }
        }

        private (IFileSystem fileSystem, string path) GetExportPathInfo()
        {
            IFileSystem fileSystem;
            string exportPath;

            switch (ConfigManager.Instance.Property.Export.Select)
            {
            case ExportSelect.Local:
                fileSystem = new LocalFileSystem();
                exportPath = ConfigManager.Instance.Property.Export.Local.Path;
                break;

            case ExportSelect.Ftp:
                fileSystem = new FtpFileSystem(
                    ConfigManager.Instance.Property.Export.Ftp.Host,
                    ConfigManager.Instance.Property.Export.Ftp.Port,
                    ConfigManager.Instance.Property.Export.Ftp.UserName,
                    ConfigManager.Instance.Property.Export.Ftp.GetPassword());
                exportPath = ConfigManager.Instance.Property.Export.Ftp.Path;
                break;

            default:
                throw new InvalidEnumArgumentException();
            }

            return (fileSystem, exportPath);
        }

        public void ImportPlaylistCommandAction()
        {
            if (_importExecute is null)
            {
                Status = RunningStatus.Importing;

                PlaylistListViewSource.Clear();
                _fileHashDictionary.Clear();

                var exportPathInfo = GetExportPathInfo();

                _importExecute = new ImportExecute()
                {
                    ImportPath = ImportPath.Value,
                    ReferencePath = ReferencePath.Value,
                    ExportPath = exportPathInfo.path,
                    FileHashDictionary = _fileHashDictionary,
                    ExportFileSystem = exportPathInfo.fileSystem,
                    PlaylistIO = PlaylistIOUtillity.CreateIPlaylistIO(ConfigManager.Instance.Property.PlaylistFormatSelect)
                };

                _importExecute.ProgressChanged += (sender, e) =>
                        SetProgress(e.ProgressPercentage, e.UserState.ToString());

                _importExecute.RunWorkerCompleted += (sender, e) =>
                        ImportPlaylistCompleted(e);

                _importExecute.RunWorkerAsync();
            }
            else
            {
                _importExecute.CancelAsync();
            }
        }

        private void ImportPlaylistCompleted(RunWorkerCompletedEventArgs e)
        {
            try
            {
                // エラーが発生した？
                if (e.Error is not null)
                {
                    Logger.GetInstance().Error(e.Error);

                    Status = RunningStatus.NoImport;
                    SetErrorProgress("エラー");
                    return;
                }

                // キャンセルされた？
                if (e.Cancelled)
                {
                    Status = RunningStatus.NoImport;
                    SetProgress(100, "キャンセルされました");
                    return;
                }

                var playlistDiffItems = (IList<PlaylistDiffItem>)e.Result;

                var isAllEqual = true;
                var hasErrorStatus = false;
                foreach (var item in playlistDiffItems)
                {
                    PlaylistListViewSource.Add(new PlaylistListViewItem(item));

                    if(item.Status != PlaylistDiffItem.DiffStatus.Equal)
                    {
                        isAllEqual = false;
                    }

                    if (item.Status == PlaylistDiffItem.DiffStatus.Error)
                    {
                        hasErrorStatus = true;
                    }
                }

                if (hasErrorStatus)
                {
                    Status = RunningStatus.NoImport;
                    SetErrorProgress("エラー");
                    return;
                }

                PlaylistSelectedIndex.Value = 0;
                Status = (isAllEqual) ? RunningStatus.Completed : RunningStatus.Ready;

                SetProgress(100, "インポートが完了しました");
            }
            finally
            {
                // _importExecuteを確実にnullにする
                _importExecute = null;
            }
        }

        public void SyncronizeCommandAction()
        {
            if (_syncronizeExecute is null)
            {
                Status = RunningStatus.Syncronizing;

                var exportPathInfo = GetExportPathInfo();

                _syncronizeExecute = new SyncronizeExecute()
                {
                    PlaylistDiffItems = PlaylistListViewSource.Select(x => x.PlaylistDiffItem).ToList(),
                    ExportPath = exportPathInfo.path,
                    ExportFileSystem = exportPathInfo.fileSystem,
                    PlaylistIO = PlaylistIOUtillity.CreateIPlaylistIO(ConfigManager.Instance.Property.PlaylistFormatSelect)
                };

                _syncronizeExecute.ProgressChanged += (sender, e) =>
                    SetProgress(e.ProgressPercentage, e.UserState.ToString());

                _syncronizeExecute.RunWorkerCompleted += (sender, e) =>
                    SyncronizeCompleted(e);

                _syncronizeExecute.RunWorkerAsync();
            }
            else
            {
                _syncronizeExecute.CancelAsync();
            }
        }


        private void SyncronizeCompleted(RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error is not null)
            {
                Status = RunningStatus.Ready;
                SetProgress(100, "キャンセルされました");
                return;
            }

            ReImportPlaylist();
        }

        private void ReImportPlaylist()
        {
            var importPlaylists = PlaylistListViewSource
                .Select(x => x.PlaylistDiffItem.Item1)
                .Where(x => x is not null)
                .ToList();

            PlaylistListViewSource.Clear();
            // 再インポートの時は_fileHashDictionaryをクリアしない

            var exportPathInfo = GetExportPathInfo();

            _syncronizeExecute = new ImportExecute()
            {
                ImportPlaylists = importPlaylists,
                ReferencePath = ReferencePath.Value,
                ExportPath = exportPathInfo.path,
                FileHashDictionary = _fileHashDictionary,
                ExportFileSystem = exportPathInfo.fileSystem,
                PlaylistIO = PlaylistIOUtillity.CreateIPlaylistIO(ConfigManager.Instance.Property.PlaylistFormatSelect)
            };

            _syncronizeExecute.ProgressChanged += (sender, e) =>
                SetProgress(e.ProgressPercentage, e.UserState.ToString());

            _syncronizeExecute.RunWorkerCompleted += (sender, e) =>
                ReImportPlaylistCompleted(e);

            _syncronizeExecute.RunWorkerAsync();
        }

        private void ReImportPlaylistCompleted(RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled || e.Error is not null)
                {
                    Status = RunningStatus.NoImport;
                    SetProgress(100, "キャンセルされました");
                    return;
                }

                foreach (var item in (IList<PlaylistDiffItem>)e.Result)
                {
                    PlaylistListViewSource.Add(new PlaylistListViewItem(item));
                }

                _fileHashDictionary = ((ImportExecute)_syncronizeExecute).FileHashDictionary;

                PlaylistSelectedIndex.Value = 0;
                Status = RunningStatus.Completed;

                SetProgress(100, "同期が完了しました");
            }
            finally
            {
                _syncronizeExecute = null;
            }
        }

        public void SelectPlaylistListViewItemCommandAction()
        {
            TrackListViewSource.Clear();

            if (PlaylistSelectedItem.Value is null)
                return;

            // ToArrayにしないと'Collection was modified; enumeration operation may not execute.'のエラーが出る
            foreach (var item in PlaylistSelectedItem.Value.PlaylistDiffItem.TrackDiffItems.ToArray())
            {
                // フィルター処理
                switch (item.Status)
                {
                case TrackDiffItem.DiffStatus.Equal:
                    if (!_isVisibleEqualTrackItem) continue;
                    break;
                case TrackDiffItem.DiffStatus.Added:
                    if (!_isVisibleAddedTrackItem) continue;
                    break;
                case TrackDiffItem.DiffStatus.Deleted:
                    if (!_isVisibleDeletedTrackItem) continue;
                    break;
                case TrackDiffItem.DiffStatus.Updated:
                    if (!_isVisibleUpdatedTrackItem) continue;
                    break;
                case TrackDiffItem.DiffStatus.Error:
                    if (!_isVisibleErrorTrackItem) continue;
                    break;
                }

                TrackListViewSource.Add(new TrackListViewItem(item));
            }
        }

        public void ClickTrackListViewColumnCommandAction()
        {
            IsVisibleEqualTrackItem.Value = _isVisibleEqualTrackItem;
            IsVisibleAddedTrackItem.Value = _isVisibleAddedTrackItem;
            IsVisibleDeletedTrackItem.Value = _isVisibleDeletedTrackItem;
            IsVisibleUpdatedTrackItem.Value = _isVisibleUpdatedTrackItem;
            IsVisibleErrorTrackItem.Value = _isVisibleErrorTrackItem;

            var counts = ImportUtility.GetTrackDiffItemStatusCount(PlaylistSelectedItem.Value?.PlaylistDiffItem.TrackDiffItems);
            VisibleEqualTrackItemText.Value = $"差分なし ({counts.nEqual})";
            VisibleAddedTrackItemText.Value = $"追加 ({counts.nAdded})";
            VisibleDeletedTrackItemText.Value = $"削除 ({counts.nDeleted})";
            VisibleUpdatedTrackItemText.Value = $"更新あり ({counts.nUpdated})";
            VisibleErrorTrackItemText.Value = $"エラー ({counts.nError})";

            IsOpenTrackStatusPopup.Value = true;
        }

        public void TrackStatusPopupOkCommandAction()
        {
            _isVisibleEqualTrackItem = IsVisibleEqualTrackItem.Value;
            _isVisibleAddedTrackItem = IsVisibleAddedTrackItem.Value;
            _isVisibleDeletedTrackItem = IsVisibleDeletedTrackItem.Value;
            _isVisibleUpdatedTrackItem = IsVisibleUpdatedTrackItem.Value;
            _isVisibleErrorTrackItem = IsVisibleErrorTrackItem.Value;

            UpdateTrackListViewStatusIcon();

            IsOpenTrackStatusPopup.Value = false;

            SelectPlaylistListViewItemCommandAction();
        }


        private IFileSystem GetExportFileSystem()
        {
            return new LocalFileSystem();
        }

        public void UpdateTrackListViewStatusIcon()
        {
            var bytes = (_isVisibleEqualTrackItem && _isVisibleAddedTrackItem
            && _isVisibleDeletedTrackItem && _isVisibleUpdatedTrackItem)
            ? Resource.filter_off : Resource.filter_on;

            TrackListViewStatusFilterIcon.Value = ResourceManager.GetBitmapImage(bytes);
        }

        public void SetErrorProgress(string text)
            => SetProgress(100, text, Color.FromRgb(255,0,0));
        public void SetProgress(int progressPercentage, string text, Color? color = null)
        {
            ProgressBarRatio.Value = progressPercentage;
            ProgressLabelRatio.Value = $"{progressPercentage}%";
            ProgressText.Value = text;

            var textColor = (color is not null)
                ? ((Color)color)
                : SystemColors.ControlTextColor;
            ProgressTextBrush.Value = new SolidColorBrush(textColor);
        }
    }
}
