using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModBotBackend
{
	[FolderName("CustomConsoleCss")]
	public class ConsoleCustomCssManager : OwnFolderObject<ConsoleCustomCssManager>
	{
		public void SetCustomCss(string userID, string css)
		{
			string path = FolderPath + userID + ".css";
			File.WriteAllText(path, css);
		}

		public void ClearCustomCssForUser(string userID)
		{
			string path = FolderPath + userID + ".css";
			if (File.Exists(path))
				File.Delete(path);
		}

		public string GetCssForUserID(string userID)
		{
			string path = FolderPath + userID + ".css";

			if (!File.Exists(path))
				return null;

			return File.ReadAllText(path);
		}

	}
}
