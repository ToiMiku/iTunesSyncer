using FluentFTP;
using iTunesSyncer.Extensions;
using System.IO;

namespace iTunesSyncer.FtpFileAccess
{
    public class FtpStream : OverrideStream
    {
        private readonly FtpClient _ftpClient;

        public FtpStream(FtpClient ftpClient, Stream stream)
            : base(stream)
        {
            _ftpClient = ftpClient;
        }

        public override void Close()
        {
            base.Close();

            // Stream操作が完了したらGetReplyで応答を確認しなければならない
            // Streamを閉じてからでないとGetReplyでタイムアウトしてしまう
            var reply = _ftpClient.GetReply();

            // 失敗した場合は例外にする
            if (!reply.Success)
                throw new IOException(reply.Message);
        }
    }
}
