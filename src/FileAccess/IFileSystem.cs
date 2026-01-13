
namespace iTunesSyncer.FileAccess
{
    public interface IFileSystem
    {
        IFileAccess FileAccess { get; }
        IDirectoryAccess DirectoryAccess { get; }
    }
}
