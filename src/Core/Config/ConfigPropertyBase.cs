using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace iTunesSyncer.Core.DataSet
{
    [Serializable]
    public class ConfigPropertyBase
    {
        [JsonInclude]
        public string Version;

        public ConfigPropertyBase()
        {
            Version = string.Empty;
        }

        public ConfigPropertyBase(Stream stream)
        {
            var deserializedObject = JsonSerializer.Deserialize<ConfigPropertyBase>(stream);

            Version = deserializedObject.Version;
        }

        public ConfigPropertyBase(ConfigPropertyBase configPropertyPre)
        {
        }
    }
}
