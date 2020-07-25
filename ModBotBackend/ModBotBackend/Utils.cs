using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend
{
	public static class Utils
	{
		public static string CombinePaths(string path1, string path2)
		{
			if (!path1.EndsWith("\\") && !path1.EndsWith("/"))
				path1 += "/";

			return path1 + path2;
		}

	}
}
