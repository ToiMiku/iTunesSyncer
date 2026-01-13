using System.Collections.Generic;
using System.IO;
using System.Text;

namespace iTunesSyncer.Core.Data
{
    public class FileHashDictionary
    {
        private Dictionary<string, string> _hashDictionary = new();

        public FileHashDictionary()
        {
        }

        public string GetHash(string filename)
        {
            if (_hashDictionary.ContainsKey(filename))
            {
                return _hashDictionary[filename];
            }

            return GetHashAndAddDictionary(filename);
        }

        private string GetHashAndAddDictionary(string filename)
        {
            using var fileStream = new FileStream(filename, FileMode.Open, System.IO.FileAccess.Read);

            // ハッシュを計算
            using var hashProvider = System.Security.Cryptography.SHA1.Create();
            var hashBytes = hashProvider.ComputeHash(fileStream);

            // ハッシュ値を文字列に変換
            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
            {
                sb.Append(hashByte.ToString("x2"));
            }
            var hashText = sb.ToString();

            _hashDictionary.Add(filename, hashText);

            return hashText;
        }

        public void Clear()
        {
            _hashDictionary.Clear();
        }
    }
}
