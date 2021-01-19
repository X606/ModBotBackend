using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend
{
	public static class OutputConsole
	{
		public static void WriteLine(string line, bool dontHttpEncode = false)
		{
			DateTime now = DateTime.Now;

			string output = "[" + now.ToString() + "] " + line;

			OnWriteLine?.Invoke(output, dontHttpEncode);
			Console.WriteLine(output);
			LogsManager.Instance.WriteLine(output);
		}

		public static void WriteLine(string line, string colorHex, bool dontHttpEncode = false)
		{
			if (!colorHex.StartsWith("#"))
				colorHex = "#" + colorHex;

			DateTime now = DateTime.Now;

			string output = "[" + now.ToString() + "] " + line;
			string htmlOutput = "";
			if (dontHttpEncode)
			{
				htmlOutput = "[" + now.ToString() + "] <span style=\"color=" + colorHex + "\">" + line + "</span>";
			} else
			{
				htmlOutput = "[" + now.ToString() + "] <span style=\"color=" + colorHex + "\">" + System.Web.HttpUtility.HtmlEncode(line) + "</span>";
			}

			OnWriteLine?.Invoke(htmlOutput, true);
			Console.WriteLine(output);
			LogsManager.Instance.WriteLine(output);
		}

		public static event Action<string, bool> OnWriteLine;
	}
}
