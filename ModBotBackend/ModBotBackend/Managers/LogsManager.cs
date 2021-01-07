using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModBotBackend
{
	[FolderName("Logs")]
	public class LogsManager : OwnFolderObject<LogsManager>
	{
		FileStream _currentLogFile = null;

		public void CreateNewLogFile()
		{
			string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

			string filename = date + ".log";

			string filepath = FolderPath + filename;

			if (_currentLogFile != null)
			{
				_currentLogFile.Close();
				_currentLogFile.Dispose();
				_currentLogFile = null;
			}

			_currentLogFile = File.Create(filepath);
		}

		public void Write(string text)
		{
			if (_currentLogFile == null)
				CreateNewLogFile();

			byte[] buffer = Encoding.UTF8.GetBytes(text);
			_currentLogFile.Write(buffer, 0, buffer.Length);
			_currentLogFile.Flush();
		}
		public void WriteLine(string line)
		{
			if (line == null)
				line = "null";

			line = line.Replace("\n", "\r\n");

			Write(line + "\r\n");
		}

	}
}
