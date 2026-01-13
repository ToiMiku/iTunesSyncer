using FluentFTP;
using iTunesSyncer.FileAccess;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace iTunesSyncer.FtpFileAccess
{
    public class FtpDirectoryAccess : IDirectoryAccess
    {
        private readonly FtpClient _ftpClient;

        public FtpDirectoryAccess(FtpClient ftpClient)
        {
            _ftpClient = ftpClient;
        }

        public void Create(string path)
        {
            _ftpClient.ConnectIfDisconnected();

            _ftpClient.CreateDirectory(path);
        }

        public void Delete(string path)
        {
            _ftpClient.ConnectIfDisconnected();

            _ftpClient.DeleteDirectory(path);
        }

        public IEnumerable<string> EnumerateDirectories(string path)
        {
            _ftpClient.ConnectIfDisconnected();

            return _ftpClient.GetListing(path)
                .Where(x => x.Type == FtpObjectType.Directory)
                .Select(x => x.FullName);
        }

        public IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            _ftpClient.ConnectIfDisconnected();

            return _ftpClient.GetListing(path)
                .Where(x => x.Type == FtpObjectType.File)
                .Where(x => IsWildcardMatch(x.Name, searchPattern))
                .Select(x => x.FullName);
        }

        public bool Exists(string path)
        {
            _ftpClient.ConnectIfDisconnected();

            return _ftpClient.DirectoryExists(path);
        }

        private bool IsWildcardMatch(string input, string pattern)
        {
            var regexPattern =
                "^"
                + Regex.Escape(pattern)
                    .Replace("\\*", ".*")
                    .Replace("\\?", ".") 
                + "$";

            return Regex.IsMatch(input, regexPattern);
        }
    }
}
