using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

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

		public static void RederectToErrorPage(HttpListenerContext context, string message, bool isError = true)
		{
			//string url = "https://clonedronemodbot.com/error.html?error=" + message;
			string url = "/errorPage.html?error=" + message;
			if (!isError)
			{
				url += "&notError=true";
			}

			Rederect(context, url);
		}
		public static void Rederect(HttpListenerContext context, string url)
		{
			context.Response.Redirect(url);
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send("re-routed");
			httpStream.Close();
		}

		public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
		{
			foreach(DirectoryInfo dir in source.GetDirectories())
				CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
			foreach(FileInfo file in source.GetFiles())
				file.CopyTo(Path.Combine(target.FullName, file.Name));
		}
		public static void RecursivelyDeleteFolder(string folder)
		{
			string[] folders = Directory.GetDirectories(folder);
			foreach(string subFolder in folders)
			{
				RecursivelyDeleteFolder(subFolder);
			}

			string[] files = Directory.GetFiles(folder);
			foreach(string file in files)
			{
				File.Delete(file);
			}

			Directory.Delete(folder);
		} 

	}
}
