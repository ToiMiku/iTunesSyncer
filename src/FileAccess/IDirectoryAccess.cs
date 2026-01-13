using System.Collections.Generic;

namespace iTunesSyncer.FileAccess
{
    public interface IDirectoryAccess
    {
        void Create(string path);

        IEnumerable<string> EnumerateDirectories(string path);

        IEnumerable<string> EnumerateFiles(string path, string searchPattern);

        bool Exists(string path);

        void Delete(string path);
    }
}
