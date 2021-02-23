using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModBotBackend
{
    public class JsonDatabase
    {
        readonly string _filePath;
        readonly DatabaseSaveOptions _options;
        readonly Dictionary<string, JToken> _data = new Dictionary<string, JToken>();

        public JsonDatabase(string filePath, DatabaseSaveOptions options)
        {
            _options = options;
            _filePath = filePath;

            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                _data = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);
            }
            else
            {
                _data = new Dictionary<string, JToken>();
                if (_options == DatabaseSaveOptions.Automatic)
                    Save();
            }

        }

        public void Set<T>(string key, T value)
        {
            _data[key] = JToken.FromObject(value);

            if (_options == DatabaseSaveOptions.Automatic)
                Save();
        }
        public T Get<T>(string key)
        {
            if (_data.TryGetValue(key, out JToken result))
            {
                return result.ToObject<T>();
            }

            return default;
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(_data);
            File.WriteAllText(_filePath, json);
        }

        public enum DatabaseSaveOptions
        {
            Automatic,
            Manual
        }

    }
}
