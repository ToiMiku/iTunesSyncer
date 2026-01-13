using iTunesSyncer.Core.Config;
using iTunesSyncer.Core.Data;
using iTunesSyncer.Diagnostics;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace iTunesSyncer.Core.DataSet
{
    [Serializable]
    public class ConfigProperty_1_2_0 : ConfigPropertyBase
    {
        [JsonInclude]
        public ImportDataSet Import = new();
        [JsonInclude]
        public ExportDataSet Export = new();
        [JsonInclude]
        public LoggerDataSet Logger = new();

        public class ImportDataSet
        {
            [JsonInclude]
            public string LocalPath = "";
            [JsonInclude]
            public string ReferencePath = "";
        }

        public class ExportDataSet
        {
            [JsonInclude]
            public ExportSelect Select = ExportSelect.Local;
            [JsonIgnore]
            public int SelectIndex
            {
                get
                {
                    return (int)Select;
                }
                set
                {
                    if (value == SelectIndex)
                        return;

                    Select = (ExportSelect)Enum.ToObject(typeof(ExportSelect), value);
                }
            }

            [JsonInclude]
            public ExportLocalDataSet Local = new();
            [JsonInclude]
            public ExportFtpDataSet Ftp = new();

            public class ExportLocalDataSet
            {
                [JsonInclude]
                public string Path = "";
            };

            public class ExportFtpDataSet
            {
                private const string FtpPasswordCryptoName = "FTP_Password";

                [JsonInclude]
                public string Host = "";
                [JsonInclude]
                public int Port = 0;
                [JsonInclude]
                public string UserName = "";

                [JsonInclude]
                public string CryptroPassword = "";

                public string GetPassword()
                {
                    if (CryptroPassword == "")
                        return "";

                    string plain;
                    try
                    {
                        plain = CryptoProcessor.Decrypt(FtpPasswordCryptoName, CryptroPassword);
                    }
                    catch (CryptoProcessor.ConsistencyException ex)
                    {
                        Diagnostics.Logger.GetInstance().Error(ex);
                        plain = "";
                    }

                    return plain;
                }
                public void SetPassword(string plain)
                {
                    CryptroPassword = (plain != "") ? CryptoProcessor.Encrypt(FtpPasswordCryptoName, plain) : "";
                }

                [JsonInclude]
                public string Path = "";
            };
        }

        public class LoggerDataSet
        {
            [JsonInclude]
            public string Path = "./";
            [JsonInclude]
            public string FileName = "diagnosis_console";
            [JsonInclude]
            public bool IsLogFile = true;
            [JsonInclude]
            public int Level = 3;
            [JsonInclude]
            public int MaxSize = 10485760;
            [JsonInclude]
            public int Period = 30;
        }

        public static readonly string MyVersion = "1.2.0";

        public ConfigProperty_1_2_0()
        {
            Version = MyVersion;
        }

        public ConfigProperty_1_2_0(Stream stream)
        {
            var deserializedObject = JsonSerializer.Deserialize<ConfigProperty_1_2_0>(stream);

            Version = deserializedObject.Version;
            Import = deserializedObject.Import;
            Export = deserializedObject.Export;
        }

        public ConfigProperty_1_2_0(ConfigPropertyBase configPropertyPre)
            : this()
        {
            throw new NotImplementedException();
        }
    }
}
