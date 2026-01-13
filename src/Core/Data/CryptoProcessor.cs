using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace iTunesSyncer.Core.Data
{
    public class CryptoProcessor
    {
        public class ConsistencyException : Exception 
        {
            public ConsistencyException() : base() { }
            public ConsistencyException(string? message, Exception? innerException) : base(message, innerException) { }
        }

        [Serializable]
        private class CryptoData
        {
            [JsonInclude]
            public string OriginalText { get; private set; }

            [JsonInclude]
            public byte[] Hash { get; private set; }

            public CryptoData(string originalText)
            {
                OriginalText = originalText;

                Hash = GetHashByteArray(originalText, 256);
            }

            public bool Consistency()
            {
                var hash = GetHashByteArray(OriginalText, 256);
                return hash.SequenceEqual(Hash);
            }
        }

        /// <summary>
        /// 平文を暗号文に変換します
        /// </summary>
        /// <param name="name">同じ平文を暗号化した時、同じ暗号文になるのを防ぐため、平文の名前を設定します</param>
        /// <param name="originalText">平文</param>
        /// <returns>暗号文</returns>
        public static string Encrypt(string name, string originalText)
        {
            var data = new CryptoData(originalText);
            var jsonText = SerializeToJson(data);

            (var key, var iv) = GetUniqueKeyAndIv(name);
            var encryptText = EncryptAes(jsonText, key, iv);

            return encryptText;
        }

        /// <summary>
        /// 暗号文を平文に変換します
        /// </summary>
        /// <param name="name">Encrypt時に設定した平文の名前</param>
        /// <param name="encryptText">暗号文</param>
        /// <returns>平文</returns>
        /// <exception cref="ConsistencyException">暗号文が改ざんされている</exception>
        public static string Decrypt(string name, string encryptText)
        {
            (var key, var iv) = GetUniqueKeyAndIv(name);

            CryptoData data;
            try
            {
                var jsonText = DecryptAes(encryptText, key, iv);
                data = DeserializeFromJson<CryptoData>(jsonText);
            }
            catch (FormatException ex)
            {
                throw new ConsistencyException(ex.Message, ex);
            }
            catch (CryptographicException ex)
            {
                throw new ConsistencyException(ex.Message, ex);
            }
            catch (JsonException ex)
            {
                throw new ConsistencyException(ex.Message, ex);
            }

            // 改ざんをチェック
            if (!data.Consistency())
                throw new ConsistencyException();

            return data.OriginalText;
        }

        private static (byte[] key, byte[] iv) GetUniqueKeyAndIv(string name)
        {
            var textArray = new string[]
            {
                DeviceInfo.getVolumeNo(),
                DeviceInfo.getBiosNo(),
                DeviceInfo.getMacAddr()
            };

            var maxLength = textArray.Max(x => x.Length);

            var sb = new StringBuilder();
            for (var i = 0; i < maxLength; i++)
            {
                for (var j = 0; j < textArray.Length; j++)
                {
                    var tIdx = textArray[j].Length * i / maxLength;
                    sb.Append(textArray[j][tIdx]);
                }
            }
            var mixText = sb.ToString();

            using var sha256 = SHA256.Create();
            var key = GetHashByteArray(mixText, 256);

            using var md5 = MD5.Create();
            var iv = GetHashByteArray(name, 128);

            return (key, iv);
        }

        static string SerializeToJson(object obj)
        {
            // JsonSerializerオブジェクトを作成
            var options = new JsonSerializerOptions
            {
                WriteIndented = true // インデントを追加して読みやすくする
            };

            // オブジェクトをJSON文字列に変換
            return JsonSerializer.Serialize(obj, options);
        }

        static T DeserializeFromJson<T>(string jsonString)
        {
            // JSON文字列を指定した型のオブジェクトに変換
            T deserializedObject = JsonSerializer.Deserialize<T>(jsonString);
            return deserializedObject;
        }

        static private string EncryptAes(string plain_text, byte[] key, byte[] iv)
        {
            // 暗号化した文字列格納用
            string encrypted_str;

            // Aesオブジェクトを作成
            using (var aes = Aes.Create())
            {
                // Encryptorを作成
                using var encryptor =
                    aes.CreateEncryptor(key, iv);
                // 出力ストリームを作成
                using MemoryStream out_stream = new();
                // 暗号化して書き出す
                using (CryptoStream cs = new(out_stream, encryptor, CryptoStreamMode.Write))
                {
                    using StreamWriter sw = new(cs);
                    // 出力ストリームに書き出し
                    sw.Write(plain_text);
                }
                // Base64文字列にする
                var result = out_stream.ToArray();
                encrypted_str = Convert.ToBase64String(result);
            }

            return encrypted_str;
        }

        static private string DecryptAes(string base64_text, byte[] key, byte[] iv)
        {
            string plain_text;

            // Base64文字列をバイト型配列に変換
            var cipher = Convert.FromBase64String(base64_text);

            // AESオブジェクトを作成
            using (Aes aes = Aes.Create())
            {
                // 復号器を作成
                using var decryptor =
                    aes.CreateDecryptor(key, iv);
                // 復号用ストリームを作成
                using MemoryStream in_stream = new(cipher);
                // 一気に復号
                using CryptoStream cs = new(in_stream, decryptor, CryptoStreamMode.Read);
                using StreamReader sr = new(cs);
                plain_text = sr.ReadToEnd();
            }
            return plain_text;
        }

        private static byte[] GetHashByteArray(string origin, int hashLength)
        {
            HashAlgorithm hashAlgorithm = null;

            try
            {
                switch (hashLength)
                {
                case 128:
                    hashAlgorithm = MD5.Create();
                    break;

                case 256:
                    hashAlgorithm = SHA256.Create();
                    break;

                default:
                    throw new ArgumentException();
                }

                var bytes = Encoding.UTF8.GetBytes(origin);
                var hash = hashAlgorithm.ComputeHash(bytes);

                return hash;
            }
            finally
            {
                if (hashAlgorithm is not null)
                {
                    hashAlgorithm.Dispose();
                }
            }
        }
    }
}
