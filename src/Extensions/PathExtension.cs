using System;
using System.IO;
using System.Text;

namespace iTunesSyncer.Extensions
{
    // Pathの拡張メソッドとして作りたかったけど、static classにはできないらしい
    public static class PathExtension
    {

        public static bool IsPathInDirectory(string relativeTo, string path)
        {
            var dir = new DirectoryInfo(path);
            var dirRelativeTo = new DirectoryInfo(relativeTo);

            while (dir.Parent != null)
            {
                if (dir.FullName.Equals(dirRelativeTo.FullName))
                    return true;

                dir = dir.Parent;
            }

            return false;
        }

        public static string UriEncode(string path)
        {
            var sb = new StringBuilder();
            var splitPath = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            for (var i = 0; i < splitPath.Length; i++)
            {
                var dirOrFileName = splitPath[i];

                if (dirOrFileName.IndexOf(Path.VolumeSeparatorChar) != -1)
                {
                    // ドライブ名だったらスルー
                    sb.Append(dirOrFileName);
                }
                else if (dirOrFileName != string.Empty)
                {
                    // URIエンコードを実施
                    sb.Append(Uri.EscapeDataString(dirOrFileName));
                }

                if (i < splitPath.Length - 1)
                {
                    sb.Append(Path.AltDirectorySeparatorChar);
                }
            }

            return sb.ToString();
        }

        public static string UriDecode(string path)
        {
            return Uri.UnescapeDataString(path);
        }
    }
}