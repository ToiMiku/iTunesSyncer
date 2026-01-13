using System.Collections.Generic;
using System.IO;

namespace iTunesSyncer.FileAccess
{
    public sealed class LocalDirectoryAccess : IDirectoryAccess
    {
        public void Create(string path)
            => Directory.CreateDirectory(path);

        public IEnumerable<string> EnumerateDirectories(string path)
            => Directory.EnumerateDirectories(path);

        public IEnumerable<string> EnumerateFiles(string path, string searchPattern)
            => Directory.EnumerateFiles(path, searchPattern);

        bool IDirectoryAccess.Exists(string path)
            => Directory.Exists(path);

        public void Delete(string path)
            => Directory.Delete(path);
    }
}
