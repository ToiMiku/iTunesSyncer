using FluentFTP;
using FluentFTP.Exceptions;

namespace iTunesSyncer.FtpFileAccess
{
    public static class FtpClientExtend
    {
        public static void ConnectIfDisconnected(this FtpClient client)
        {
            if (client.IsConnected)
                return;

            try
            {
                client.Connect();
            }
            catch (FtpAuthenticationException ex)
            {
                throw new System.IO.IOException(ex.Message, ex);
            }
        }
    }
}
