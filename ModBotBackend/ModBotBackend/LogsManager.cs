using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModBotBackend
{
	public static class LogsManager
	{
		public static string LogsFolderPath => Program.LogsFolderPath;

		static FileStream _currentLogFile = null;

		public static void CreateNewLogFile()
		{
			string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

			string filename = date + ".log";

			string filepath = LogsFolderPath + filename;

			if (_currentLogFile != null)
			{
				_currentLogFile.Close();
				_currentLogFile.Dispose();
				_currentLogFile = null;
			}

			_currentLogFile = File.Create(filepath);
		}

		public static void Write(string text)
		{
			if (_currentLogFile == null)
				CreateNewLogFile();

			byte[] buffer = Encoding.UTF8.GetBytes(text);
			_currentLogFile.Write(buffer, 0, buffer.Length);
			_currentLogFile.Flush();
		}
		public static void WriteLine(string line)
		{
			line = line.Replace("\n", "\r\n");

			Write(line + "\r\n");
		}

	}
}
