using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace iTunesSyncer.Core.DataSet
{
    public class ConfigManagerBase<T>
        where T : ConfigPropertyBase, new()
    {
        public readonly string FilePath = "./config.dat";

        protected List<Type> ConfigPropertyTypeList { get; } = new();

        public T Property { get; private set; }

        protected void Load()
        {
            if (!File.Exists(FilePath))
            {
                Property = new T();
                return;
            }

            using var fs = new FileStream(FilePath, FileMode.Open, System.IO.FileAccess.Read);

            var configPropertyBase = new ConfigPropertyBase(fs);

            ConfigPropertyBase currentConfigProperty = null;
            for (var i = 0; i < ConfigPropertyTypeList.Count; i++)
            {
                var configPropertyTypeItem = ConfigPropertyTypeList[i];

                if (currentConfigProperty is null)
                {
                    var fi = configPropertyTypeItem.GetField("MyVersion");
                    var itemVersion = (string)fi.GetValue(null);

                    // 保存されているConfigのバージョンに合わせて読み込む
                    if (configPropertyBase.Version == itemVersion)
                    {
                        fs.Seek(0, SeekOrigin.Begin);

                        currentConfigProperty = (ConfigPropertyBase)JsonSerializer.Deserialize(fs, configPropertyTypeItem);
                    }
                }
                else
                {
                    // バージョンが古かったら、順番に最新バージョンに変換する
                    currentConfigProperty = (ConfigPropertyBase)Activator.CreateInstance(
                            configPropertyTypeItem, new object[] { currentConfigProperty });
                }
            }

            if (currentConfigProperty is null)
            {
                // Configファイルがないので空の最新のConfigをセットする
                currentConfigProperty = (ConfigPropertyBase)Activator.CreateInstance(
                            ConfigPropertyTypeList.Last(), new object[] { });
            }

            Property = (T)currentConfigProperty;
        }

        public void Save()
        {
            // オブジェクトをJSON文字列に変換
            var jsonText = JsonSerializer.Serialize(Property, new JsonSerializerOptions()
            {
                WriteIndented = true // インデントを追加して読みやすくする
            });

            // JSON文字列をStreamに書き込み
            using var fs = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write);
            using var writer = new StreamWriter(fs);
            writer.Write(jsonText);
        }
    }
}
