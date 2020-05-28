using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ModBotBackend
{
	public class JsonFile<T>
	{
		public T Value;
		string _filePath;

		public JsonFile(string rootPath, string fileName)
		{
			if (!rootPath.EndsWith("/") && !rootPath.EndsWith("\\")) // make sure the path has a / or a \ at the end of it
				rootPath += "/";

			if (!Directory.Exists(rootPath))
				Directory.CreateDirectory(rootPath);

			_filePath = rootPath + fileName;

			if (!File.Exists(_filePath))
				File.Create(_filePath);

		}

		public void Load()
		{
			string json = File.ReadAllText(_filePath);
			Value = JsonConvert.DeserializeObject<T>(json);
		}
		public void Save()
		{
			string json = JsonConvert.SerializeObject(Value);
			File.WriteAllText(_filePath, json);
		}

	}
}
