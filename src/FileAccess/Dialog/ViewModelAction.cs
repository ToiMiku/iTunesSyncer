using iTunesSyncer.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Path = System.IO.Path;

namespace iTunesSyncer.FileAccess.Dialog
{
    public partial class ViewModel
    {
        private const string RootDirectory = "";

        private void FormLoadedCommandAction()
        {
            try
            {
                // 初期ディレクトリが存在するか確認
                if (!_fileSystem.DirectoryAccess.Exists(CurrentDirectoryPath.Value))
                {
                    // 初期ディレクトリが存在しないならルートディレクトリにアクセスできないか確認
                    if (!_fileSystem.DirectoryAccess.Exists(RootDirectory))
                    {
                        // TODO: 初期ディレクトリとルートどちらにもアクセスできないなら失敗。エラーメッセージ出したい
                        return;
                    }

                    // カレントディレクトリをルートに移動
                    CurrentDirectoryPath.Value = RootDirectory;
                }
            }
            catch (IOException ex)
            {
                ShowErrorMessageAndClose(ex);
                return;
            }

            UpdateCurrentDirectory();
        }

        private void MoveParentDirectoryCommandAction()
        {
            var newCurrentDirectory = Path.GetDirectoryName(CurrentDirectoryPath.Value);

            CurrentDirectoryPath.Value = newCurrentDirectory;
            UpdateCurrentDirectory();
        }

        private void UpdateCurrentDirectoryCommandAction()
        {

            try
            {
                if (!_fileSystem.DirectoryAccess.Exists(CurrentDirectoryPath.Value))
                {
                    MessageBox.Show(CurrentDirectoryPath.Value + "が見つかりませんでした");
                    return;
                }
            }
            catch (IOException ex)
            {
                ShowErrorMessageAndClose(ex);
                return;
            }

            UpdateCurrentDirectory();
        }

        private void CreateDirectoryCommandAction()
        {
            // ディレクトリを作成するダイアログを表示
            var dialog = new AddDirectoryDialog();
            dialog.ShowDialog();

            if (dialog.DialogResult != true)
                return;

            var newDirectoryName = 　Path.Combine(CurrentDirectoryPath.Value, dialog.DirectoryName);

            try
            {
                // 既に存在していないか？
                if (_fileSystem.DirectoryAccess.Exists(newDirectoryName))
                {
                    MessageBox.Show($"{newDirectoryName}は既に存在しています", _window.Title, MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                // ディレクトリを作成する処理
                _fileSystem.DirectoryAccess.Create(newDirectoryName);

                // 作成できたか確認
                if (!_fileSystem.DirectoryAccess.Exists(newDirectoryName))
                {
                    MessageBox.Show($"{newDirectoryName}の作成に失敗しました", _window.Title, MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
            }
            catch (IOException ex)
            {
                ShowErrorMessageAndClose(ex);
                return;
            }

            // リストを更新
            UpdateCurrentDirectory();
        }

        private void DeleteItemCommandAction()
        {
            if (FileListSelectedItem.Value is null)
                return;

            var deleteItemName = Path.Combine(CurrentDirectoryPath.Value, FileListSelectedItem.Value.Name.Value);

            var ret = MessageBox.Show($"{deleteItemName}を削除してもよろしいですか？", _window.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (ret != MessageBoxResult.OK)
                return;

            // ディレクトリの削除
            try
            {
                _fileSystem.DirectoryAccess.Delete(deleteItemName);
            }
            catch (IOException ex)
            {
                ShowErrorMessageAndClose(ex);
                return;
            }

            // リストを更新
            UpdateCurrentDirectory();
        }

        private void ListItemSelectionChangedCommandAction()
        {
            if (FileListSelectedItem.Value is null)
            {
                IsEnabledSelectedItem.Value = false;
                FileOrDirectoryName.Value = string.Empty;
                return;
            }

            if (FileListSelectedItem.Value.Type == FileListViewItem.FileObjectType.None)
            {
                IsEnabledSelectedItem.Value = false;
                FileOrDirectoryName.Value = string.Empty;
                return;
            }

            IsEnabledSelectedItem.Value = true;
            FileOrDirectoryName.Value = FileListSelectedItem.Value.Name.Value;
        }

        private void ListItemMouseDoubleClickCommandAction()
        {
            if (FileListSelectedItem.Value is null)
                return;

            if (FileListSelectedItem.Value.Type == FileListViewItem.FileObjectType.None)
                return;

            var newCurrentDirectory = Path.Combine(CurrentDirectoryPath.Value, FileListSelectedItem.Value.Name.Value);

            CurrentDirectoryPath.Value = newCurrentDirectory;
            UpdateCurrentDirectory();
        }

        private void OkCommandAction()
        {
            _window.DialogResult = true;
            _window.Close();
        }

        private void CancelCommandAction()
        {
            _window.DialogResult = false;
            _window.Close();
        }

        private void UpdateCurrentDirectory()
        {
            // アクセス中はUIの操作を無効化
            SetEnabledUI(false);

            // ディレクトリとファイルのリストを取得
            string[] directoryArray;
            string[] fileArray;
            try
            {
                directoryArray = _fileSystem.DirectoryAccess.EnumerateDirectories(CurrentDirectoryPath.Value).ToArray();
                fileArray = _fileSystem.DirectoryAccess.EnumerateFiles(CurrentDirectoryPath.Value, "*.*").ToArray();
            }
            catch (IOException ex)
            {
                ShowErrorMessageAndClose(ex);
                return;
            }

            // リストを更新
            UpdateListItem(directoryArray, fileArray);

            // UIの操作を有効化
            SetEnabledUI(true);
        }

        private void UpdateListItem(IEnumerable<string> directories, IEnumerable<string> files)
        {
            // ファイルリストの更新
            FileListViewItemSource.Clear();

            // ディレクトリを追加
            foreach (var fullpath in directories)
            {
                var directoryName = Path.GetFileName(fullpath);
                var item = new FileListViewItem(directoryName, FileListViewItem.FileObjectType.Directory);
                FileListViewItemSource.Add(item);
            }

            // ファイルを追加
            if (!IsDirectoryPicker)
            {
                foreach (var fullpath in files)
                {
                    var fileName = Path.GetFileName(fullpath);
                    var item = new FileListViewItem(fileName, FileListViewItem.FileObjectType.File);
                    FileListViewItemSource.Add(item);
                }
            }

            // リストが空だったら説明文を追加
            if (!FileListViewItemSource.Any())
            {
                FileListViewItem item;

                if (!files.Any())
                {
                    // ディレクトリもファイルもない
                    item = new FileListViewItem("空のディレクトリです", FileListViewItem.FileObjectType.None);
                }
                else
                {
                    // ディレクトリはないがファイルはある
                    item = new FileListViewItem("ディレクトリはありません", FileListViewItem.FileObjectType.None);
                }

                FileListViewItemSource.Add(item);
            }
        }

        private void SetEnabledUI(bool enabled)
        {
            if (enabled)
            {
                var isExistParent = !string.IsNullOrEmpty(Path.GetDirectoryName(CurrentDirectoryPath.Value));
                IsEnabledParentDirectory.Value = isExistParent;
            }
            else
            {
                IsEnabledParentDirectory.Value = false;
            }

            IsEnabledForAccess.Value = enabled;
        }

        private void ShowErrorMessageAndClose(Exception e)
        {
            Logger.GetInstance().Error(e);

            MessageBox.Show("アクセスできませんでした。", _window.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            _window.DialogResult = false;
            _window.Close();
        }
    }
}
