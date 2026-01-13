# iTunesSyncer

iTunesで管理しているプレイリスト情報を元に、
楽曲ファイルとプレイリストを同期する Windows 向けツールです。

<img width="1097" height="631" alt="image" src="https://github.com/user-attachments/assets/5c12c05e-88fb-4fe9-9707-12b76c55ea48" />


## ダウンロード
[Releases](../../releases) から最新版をダウンロードしてください。

## 使用方法
1. iTunesでプレイリストを作成
2. iTunesSyncer.exe を起動
3. iTunesプレイリストのインポート元のパスを設定（通常は変更不要）
4. 音楽ファイルが保存されている上位のパスを設定
5. エクスポート先を指定
　　ローカルフォルダとFTPから選択可能
6. インポートを実行すると、インポート元とエクスポート先のファイルを比較し
　　差分を表示します
7. 差分内容を確認し、同期を実行

## エクスポート先設定
1. エクスポート先にはローカルフォルダとFTPから選択可能
2. プレイリストフォーマットは以下から選択可能
　・M3U
　・M3U8
　・XSPF

<img width="697" height="499" alt="image" src="https://github.com/user-attachments/assets/06892107-6555-4ef3-96b8-0f55f051053f" />

## 注意事項
・本ツールは iTunes の管理情報を変更しません
・コピー処理のみを行います
・楽曲ファイルの権利・利用については各自の責任でお願いします


## 動作環境
- Windows 10で動作確認済み

## ライセンス
MIT License
