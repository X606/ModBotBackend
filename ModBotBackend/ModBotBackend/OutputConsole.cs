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
			LogsManager.WriteLine(output);
		}

		public static event Action<string, bool> OnWriteLine;
	}
}
