using iTunesSyncer.Core.Data;
using iTunesSyncer.FileAccess;
using iTunesSyncer.PlaylistUtility;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace iTunesSyncer.Core
{
    public class SyncronizeExecute : MyBackgroundWorker
    {
        public IList<PlaylistDiffItem> PlaylistDiffItems { get; set; }
        public string ExportPath { get; set; }
        public IFileSystem ExportFileSystem { get; set; }
        public IPlaylistIO PlaylistIO { get; set; }



        public SyncronizeExecute() { }

        protected override object? DoWorkTask(object sender, DoWorkEventArgs e)
        {
            ReportProgress(0, "プレイリストの同期を開始しています...");

            // コピーまたは削除が必要なファイルのチェック
            (var copyTracks, var deleteTracks) = SyncronizeUtility.GetCopyAndDeleteTracks(
                PlaylistDiffItems.SelectMany(x => x.TrackDiffItems));

            // コピー・削除の実行
            CopyAndDeleteFiles(copyTracks, deleteTracks);

            // プレイリストを同期する
            var num = PlaylistDiffItems.Count;
            var i = 0;
            foreach (var playlistDiffItem in PlaylistDiffItems)
            {
                ReportProgress(100 * i / num, playlistDiffItem.Item1.Title + "の同期中...");
                i++;

                SyncronizeUtility.SyncronizePlaylist(playlistDiffItem, ExportPath, ExportFileSystem, PlaylistIO);
                ThrowIfCanceled();
            }

            return null;
        }

        private void CopyAndDeleteFiles(IList<Track> copyTracks, IList<Track> deleteTracks)
        {
            var i = 0;
            var num = copyTracks.Count + deleteTracks.Count;

            // コピーの実行
            foreach (var track1 in copyTracks)
            {
                ReportProgress(100 * i / num, track1.Title + "をコピー中...");
                i++;

                SyncronizeUtility.CopyTrack(track1, ExportPath, ExportFileSystem);
                ThrowIfCanceled();
            }

            // 削除の実行
            foreach (var track2 in deleteTracks)
            {
                ReportProgress(100 * i / num, track2.Title + "を削除中...");
                i++;

                SyncronizeUtility.DeleteTrack(track2, ExportPath, ExportFileSystem);
                ThrowIfCanceled();
            }
        }
    }
}
