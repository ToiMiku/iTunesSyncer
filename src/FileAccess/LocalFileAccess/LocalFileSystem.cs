
namespace iTunesSyncer.FileAccess
{
    public class LocalFileSystem : IFileSystem
    {
        public IFileAccess FileAccess { get; private set; }

        public IDirectoryAccess DirectoryAccess { get; private set; }

        public LocalFileSystem()
        {
            FileAccess = new LocalFileAccess();
            DirectoryAccess = new LocalDirectoryAccess();
        }
    }
}
