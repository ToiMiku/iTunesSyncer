using iTunesSyncer.Core.Data;
using iTunesSyncer.FileAccess;
using iTunesSyncer.PlaylistUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace iTunesSyncer.Core
{
    public class ImportExecute : MyBackgroundWorker
    {
        public string ImportPath { get; set; }
        public IList<Playlist> ImportPlaylists { get; set; }
        public string ReferencePath { get; set; }
        public string ExportPath { get; set; }
        public IFileSystem ExportFileSystem { get; set; }
        public FileHashDictionary FileHashDictionary { get; set; }
        public IPlaylistIO PlaylistIO { get; set; }


        public ImportExecute() { }

        protected override object? DoWorkTask(object sender, DoWorkEventArgs e)
        {
            // iTunesからプレイリストをインポート
            IList<Playlist> importPlaylits;
            if (ImportPath is not null)
            {
                ReportProgress(0, "iTunesからプレイリストをインポート中...");
                importPlaylits = ImportUtility.LoadPlaylistsFromITunes(ImportPath);
                ImportUtility.UpdateRelativePathInPlaylists(importPlaylits, ReferencePath);
            }
            else if (ImportPlaylists is not null)
            {
                // インポート済みプレイリストを使用する
                importPlaylits = ImportPlaylists;
            }
            else
            {
                throw new ArgumentException();
            }
            ThrowIfCanceled();

            // エクスポート先からプレイリストをインポート
            ReportProgress(0, "エクスポート先からプレイリストをインポート中...");
            var exportPlaylists = ImportUtility.LoadExportPlaylists(ExportPath, ExportFileSystem, PlaylistIO);
            ThrowIfCanceled();

            // プレイリストを比較して差分リストを作成
            var playlistDiffList = ImportUtility.GeneratePlaylistDiffList(importPlaylits, exportPlaylists);
            ThrowIfCanceled();

            // 各プレイリストのトラックを取得
            foreach (var playlistDiffItem in playlistDiffList)
            {
                // トラックを比較して差分リストを作成
                var trackDiffItemList = ImportUtility.GenerateTrackDiffList(
                    playlistDiffItem.Item1?.Tracks,
                    playlistDiffItem.Item2?.Tracks);

                playlistDiffItem.TrackDiffItems = trackDiffItemList;

                ThrowIfCanceled();
            }

            // 各プレイリストのトラックの差分を判定
            foreach (var playlistDiffItem in playlistDiffList)
            {
                var trackNum = playlistDiffItem.TrackDiffItems.Count;
                var i = 0;
                foreach (var trackDiffItem in playlistDiffItem.TrackDiffItems)
                {
                    // 進捗状況の通知
                    ReportProgress(100 * i / trackNum, trackDiffItem.NotNullItem.Title + "の変更を確認中...");
                    i++;

                    // トラックのStatusを更新
                    ImportUtility.UpdateTrackDiffItemStatus(trackDiffItem, FileHashDictionary);

                    ThrowIfCanceled();
                }

                // プレイリストのStatusを更新
                ImportUtility.UpdatePlaylistDiffItemStatus(playlistDiffItem);
            }

            return playlistDiffList;
        }
    }
}
