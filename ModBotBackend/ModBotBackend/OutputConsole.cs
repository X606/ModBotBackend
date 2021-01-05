using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend
{
	public static class OutputConsole
	{
		public static void WriteLine(string line)
		{
			OnWriteLine?.Invoke(line);
			Console.WriteLine(line);
		}

		public static event Action<string> OnWriteLine;
	}
}
