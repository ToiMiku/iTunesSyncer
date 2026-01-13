using System;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using iTunesSyncer.Core.Config;

namespace iTunesSyncer.Diagnostics
{
    public class Logger
    {
        /// <summary>
        /// ログレベル
        /// </summary>
        private enum LogLevel
        {
            ERROR,
            WARN,
            INFO,
            DEBUG
        }

        private static Logger singleton = null;
        private readonly string logFilePath = null;
        private readonly object lockObj = new object();
        private StreamWriter stream = null;

        private string DirectoryPath
            => ConfigManager.Instance.Property.Logger.Path;
        private string FileName
            => ConfigManager.Instance.Property.Logger.FileName;
        private int Level
            => ConfigManager.Instance.Property.Logger.Level;
        private bool IsLogFile
            => ConfigManager.Instance.Property.Logger.IsLogFile;
        private int MaxSize
            => ConfigManager.Instance.Property.Logger.MaxSize;
        private int Period
            => ConfigManager.Instance.Property.Logger.Period;

        /// <summary>
        /// インスタンスを生成する
        /// </summary>
        public static Logger GetInstance()
        {
            if (singleton == null)
            {
                singleton = new Logger();
            }
            return singleton;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Logger()
        {
            this.logFilePath = DirectoryPath + FileName + ".log";

            // ログファイルを生成する
            CreateLogfile(new FileInfo(logFilePath));
        }

        /// <summary>
        /// ERRORレベルのログを出力する
        /// </summary>
        /// <param name="msg">メッセージ</param>
        public void Error(string msg)
        {
            if ((int)LogLevel.ERROR <= Level)
            {
                Out(LogLevel.ERROR, msg);
            }
        }

        /// <summary>
        /// ERRORレベルのスタックトレースログを出力する
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        public void Error(Exception ex)
        {
            if ((int)LogLevel.ERROR <= Level)
            {
                if (ex.InnerException is not null)
                {
                    Error(ex.InnerException);
                }

                Out(LogLevel.ERROR,
                    ex.Message + Environment.NewLine +
                    ex.GetType().FullName + Environment.NewLine +
                    ex.StackTrace);
            }
        }

        /// <summary>
        /// WARNレベルのログを出力する
        /// </summary>
        /// <param name="msg">メッセージ</param>
        public void Warn(string msg)
        {
            if ((int)LogLevel.WARN <= Level)
            {
                Out(LogLevel.WARN, msg);
            }
        }

        /// <summary>
        /// INFOレベルのログを出力する
        /// </summary>
        /// <param name="msg">メッセージ</param>
        public void Info(string msg)
        {
            if ((int)LogLevel.INFO <= Level)
            {
                Out(LogLevel.INFO, msg);
            }
        }

        /// <summary>
        /// DEBUGレベルのログを出力する
        /// </summary>
        /// <param name="msg">メッセージ</param>
        public void Debug(string msg)
        {
            if ((int)LogLevel.DEBUG <= Level)
            {
                Out(LogLevel.DEBUG, msg);
            }
        }

        /// <summary>
        /// ログを出力する
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="msg">メッセージ</param>
        private void Out(LogLevel level, string msg)
        {
            if (IsLogFile)
            {
                int tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
                string fullMsg = string.Format("[{0}][{1}][{2}] {3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tid, level.ToString(), msg);

                lock (this.lockObj)
                {
                    this.stream.WriteLine(fullMsg);

                    FileInfo logFile = new FileInfo(this.logFilePath);
                    if (MaxSize < logFile.Length)
                    {
                        // ログファイルを圧縮する
                        CompressLogFile();

                        // 古いログファイルを削除する
                        DeleteOldLogFile();
                    }
                }
            }
        }

        /// <summary>
        /// ログファイルを生成する
        /// </summary>
        /// <param name="logFile">ファイル情報</param>
        private void CreateLogfile(FileInfo logFile)
        {
            if (!Directory.Exists(logFile.DirectoryName))
            {
                Directory.CreateDirectory(logFile.DirectoryName);
            }

            this.stream = new StreamWriter(logFile.FullName, true, Encoding.UTF8)
            {
                AutoFlush = true
            };
        }

        /// <summary>
        /// ログファイルを圧縮する
        /// </summary>
        private void CompressLogFile()
        {
            this.stream.Close();
            var oldFilePath = DirectoryPath + FileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            File.Move(this.logFilePath, oldFilePath + ".log");

            var inStream = new FileStream(oldFilePath + ".log", FileMode.Open, System.IO.FileAccess.Read);
            var outStream = new FileStream(oldFilePath + ".gz", FileMode.Create, System.IO.FileAccess.Write);
            var gzStream = new GZipStream(outStream, CompressionMode.Compress);

            var size = 0;
            var buffer = new byte[MaxSize + 1000];
            while (0 < (size = inStream.Read(buffer, 0, buffer.Length)))
            {
                gzStream.Write(buffer, 0, size);
            }

            inStream.Close();
            gzStream.Close();
            outStream.Close();

            File.Delete(oldFilePath + ".log");
            CreateLogfile(new FileInfo(this.logFilePath));
        }

        /// <summary>
        /// 古いログファイルを削除する
        /// </summary>
        private void DeleteOldLogFile()
        {
            Regex regex = new Regex(FileName + @"_(\d{14}).*\.gz");
            DateTime retentionDate = DateTime.Today.AddDays(-Period);
            string[] filePathList = Directory.GetFiles(DirectoryPath, FileName + "_*.gz", SearchOption.TopDirectoryOnly);
            foreach (string filePath in filePathList)
            {
                Match match = regex.Match(filePath);
                if (match.Success)
                {
                    DateTime logCreatedDate = DateTime.ParseExact(match.Groups[1].Value.ToString(), "yyyyMMddHHmmss", null);
                    if (logCreatedDate < retentionDate)
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }
    }
}
