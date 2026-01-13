using iTunesSyncer.FileAccess;
using System.IO;

namespace iTunesSyncer.Core
{
    public class FileControl
    {
        public static Stream OpenRead(string oath, IFileSystem fileSystem)
        {
            return fileSystem.FileAccess.OpenRead(oath);
        }

        public static Stream OpenWriteCreateMode(string path, IFileSystem fileSystem)
        {
            fileSystem.FileAccess.Delete(path);
            return fileSystem.FileAccess.OpenWrite(path);
        }

        public static void CreateDirAndCopyFile(string sourceFullPath, string destFullPath, IFileSystem destFileSystem)
        {
            var destParentDir = Path.GetDirectoryName(destFullPath);
            if (!destFileSystem.DirectoryAccess.Exists(destParentDir))
            {
                destFileSystem.DirectoryAccess.Create(destParentDir);
            }

            using var readStream = new FileInfo(sourceFullPath).OpenRead();
            using var writeStream = OpenWriteCreateMode(destFullPath, destFileSystem);

            readStream.CopyTo(writeStream);
            writeStream.Flush();
        }

        internal static void DeleteFile(string outputPath, IFileSystem exportFileSystem)
        {
            exportFileSystem.FileAccess.Delete(outputPath);
        }


    }
}
