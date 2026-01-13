using iTunesSyncer.Core.Config;
using Reactive.Bindings;
using System;
using System.IO;
using System.Windows.Data;

namespace iTunesSyncer.UI.MainWindow
{
    public partial class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            BindingOperations.EnableCollectionSynchronization(PlaylistListViewSource, new object());
            BindingOperations.EnableCollectionSynchronization(TrackListViewSource, new object());

            InitUIValue();
            InitCommands();

        }

        public void InitUIValue()
        {
            if (string.IsNullOrEmpty(ConfigManager.Instance.Property.Import.LocalPath))
            {
                ImportPath.Value = Path.Combine(new string[]
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                    "iTunes",
                    "iTunes Music Library.xml"
                });
            }
            else
            {
                ImportPath.Value = ConfigManager.Instance.Property.Import.LocalPath;
            }

            ReferencePath.Value = ConfigManager.Instance.Property.Import.ReferencePath;

            ExportSelectedIndex.Value = ConfigManager.Instance.Property.Export.SelectIndex;
            UpdateReadOnlyExportPath();

            Status = RunningStatus.NoImport;
            UpdateTrackListViewStatusIcon();
            SetProgress(0, "レディ");
        }

        private void InitCommands()
        {
            // InputGroup
            OpenImportPathCommand = new ReactiveCommand(IsEnabledInputGroup)
                .WithSubscribe(OpenImportPathCommandAction);

            OpenReferencePathCommand = new ReactiveCommand(IsEnabledInputGroup)
                .WithSubscribe(OpenReferencePathCommandAction);

            ExportSelectedChangedCommand = new ReactiveCommand(IsEnabledInputGroup)
                .WithSubscribe(ExportSelectedChangedCommandAction);

            OpenExportSettingCommand = new ReactiveCommand(IsEnabledInputGroup)
                .WithSubscribe(OpenExportSettingCommandAction);

            // Import
            ImportPlaylistCommand = new ReactiveCommand(IsEnabledmportButton)
                .WithSubscribe(ImportPlaylistCommandAction);

            // ListView
            PlaylistSelectedChangedCommand = new ReactiveCommand()
                .WithSubscribe(SelectPlaylistListViewItemCommandAction);

            // TrackListView Popup
            ClickTrackListViewColumnCommand = new ReactiveCommand()
                .WithSubscribe(ClickTrackListViewColumnCommandAction);

            TrackStatusPopupOkCommand = new ReactiveCommand()
                .WithSubscribe(TrackStatusPopupOkCommandAction);

            // Syncronize
            SyncronizeCommand = new ReactiveCommand(IsEnabledSyncronizeButton)
                .WithSubscribe(SyncronizeCommandAction);

        }


    }
}
