using iTunesSyncer.Core.Config;
using iTunesSyncer.Core.Data;
using iTunesSyncer.Diagnostics;
using iTunesSyncer.PlaylistUtility;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace iTunesSyncer.Core.DataSet
{
    [Serializable]
    public class ConfigProperty_1_3_0 : ConfigPropertyBase
    {
        [JsonInclude]
        public ImportDataSet Import = new();
        [JsonInclude]
        public ExportDataSet Export = new();
        [JsonInclude]
        public LoggerDataSet Logger = new();

        [JsonInclude]
        public FormatSelect PlaylistFormatSelect = FormatSelect.M3U;
        [JsonIgnore]
        public int PlaylistFormatSelectIndex
        {
            get
            {
                return (int)PlaylistFormatSelect;
            }
            set
            {
                if (value == PlaylistFormatSelectIndex)
                    return;

                PlaylistFormatSelect = (FormatSelect)Enum.ToObject(typeof(FormatSelect), value);
            }
        }

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

        public static readonly string MyVersion = "1.3.0";

        public ConfigProperty_1_3_0()
        {
            Version = MyVersion;
        }

        public ConfigProperty_1_3_0(Stream stream)
        {
            var deserializedObject = JsonSerializer.Deserialize<ConfigProperty_1_3_0>(stream);

            Version = deserializedObject.Version;
            Import = deserializedObject.Import;
            Export = deserializedObject.Export;
        }

        public ConfigProperty_1_3_0(ConfigPropertyBase configPropertyPre)
            : this()
        {
            // 非対応の過去バージョンならエラーにする
            var configPre = (ConfigProperty_1_2_0)configPropertyPre;

            Version = MyVersion;

            Import.LocalPath = configPre.Import.LocalPath;
            Import.ReferencePath = configPre.Import.ReferencePath;

            Export.Select = configPre.Export.Select;
            Export.Local.Path = configPre.Export.Local.Path;
            Export.Ftp.Host = configPre.Export.Ftp.Host;
            Export.Ftp.Port = configPre.Export.Ftp.Port;
            Export.Ftp.UserName = configPre.Export.Ftp.UserName;
            Export.Ftp.CryptroPassword = configPre.Export.Ftp.CryptroPassword;
            Export.Ftp.Path = configPre.Export.Ftp.Path;

            Logger.Path = configPre.Logger.Path;
            Logger.FileName = configPre.Logger.FileName;
            Logger.IsLogFile = configPre.Logger.IsLogFile;
            Logger.Level = configPre.Logger.Level;
            Logger.MaxSize = configPre.Logger.MaxSize;
            Logger.Period = configPre.Logger.Period;

            PlaylistFormatSelect = FormatSelect.XSPF;
        }
    }
}
