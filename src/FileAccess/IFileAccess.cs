using System.IO;

namespace iTunesSyncer.FileAccess
{
    public interface IFileAccess
    {
        /// <summary>
        /// Open + ReadとしてStreamを生成します。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Stream OpenRead(string path);

        /// <summary>
        /// OpenOrCreate + WriteとしてStreamを生成します。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Stream OpenWrite(string path);

        bool Exists(string path);

        void Delete(string path);
    }
}
