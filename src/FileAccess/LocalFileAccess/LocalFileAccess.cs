using System.IO;

namespace iTunesSyncer.FileAccess
{
    public sealed class LocalFileAccess : IFileAccess
    {
        public Stream OpenRead(string path)
            => File.OpenRead(path);

        public Stream OpenWrite(string path)
            => File.OpenWrite(path);

        public bool Exists(string path)
            => File.Exists(path);

        public void Delete(string path)
            => File.Delete(path);
    }
}
