using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace ModBotBackend
{
	public static class WebsiteRequestProcessor
	{

		static Dictionary<string, string> _fileExstensionCache = new Dictionary<string, string>();

		public static void OnRequest(HttpListenerContext context)
		{
			string path = context.Request.Url.AbsolutePath;
			path = path.TrimEnd('/', '\\');

			if(path == "")
			{
				path = "/index";
			}

			string fullPath = GetWebsiteFilePath() + path;

			string fileExtension = Path.GetExtension(path);

			if(fileExtension == "")
			{
				if(_fileExstensionCache.TryGetValue(fullPath, out string chachedExtsion))
				{
					fullPath += chachedExtsion;
					fileExtension = Path.GetExtension(fullPath);
				}
				else
				{



					string[] extensions = new string[]
					{
					".html",
					".js",
					".css",
					".png",
					".jpg",
					".jpeg"
					};

					foreach(string extension in extensions)
					{
						if(Utils.FileExistsCached(fullPath + extension))
						{
							_fileExstensionCache.Add(fullPath, extension);
							fullPath += extension;
							fileExtension = Path.GetExtension(fullPath);
							break;
						}
					}
				}
			}

			bool is404 = false;

			if(!Utils.FileExistsCached(fullPath))
			{
				is404 = true;
			}

			if(is404 && Directory.Exists(fullPath))
			{
				is404 = false;

				string page = "<head><title>Browser</title></head><body>";
				page += "<a href=\"./../\">[<--]</a><br><br>";

				string[] subFolders = Directory.GetDirectories(fullPath);
				foreach(string folder in subFolders)
				{
					page += string.Format("<a href=\"{0}\">{1}</a><br>", path + "/" + Path.GetFileName(folder), folder);
				}
				page += "<br>";

				string[] files = Directory.GetFiles(fullPath);
				foreach(string file in files)
				{
					page += string.Format("<a href=\"{0}\">{1}</a><br>", path + "/" + Path.GetFileName(file), file);
				}

				page += "</body>";

				context.Response.ContentType = "text/html";
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(page);
				stream.Close();

				return;
			}

			if(is404)
			{
				context.Response.ContentType = "text/plain";
				HttpStream stream404 = new HttpStream(context.Response);
				stream404.Send("404 :(");
				stream404.Close();
				return;
			}

			context.Response.ContentType = GetContentTypeForExtension(fileExtension);

			byte[] data = Utils.FileReadAllBytesCached(fullPath);
			if(context.Response.OutputStream.CanWrite)
			{
				context.Response.OutputStream.Write(data, 0, data.Length);
				context.Response.OutputStream.Close();
			}
			context.Response.Close();
		}

		public static string GetWebsiteFilePath()
		{
			string path = Directory.GetCurrentDirectory() + "/Website";
			string overrideFilePath = Directory.GetCurrentDirectory() + "/Website.txt";
			if(Utils.FileExistsCached(overrideFilePath))
			{
				path = Utils.FileReadAllTextCached(overrideFilePath);
				path = path.TrimEnd('/', '\\');
			}

			return path;
		}
		public static string GetContentTypeForExtension(string fileExtension)
		{
			switch(fileExtension)
			{
				case ".html":
				return "text/html";
				case ".js":
				return "application/javascript";
				case ".css":
				return "text/css";
				case ".png":
				return "image/png";
				case ".jpg":
				case ".jpeg":
				return "image/jpeg";
				case ".gif":
				return "image/gif";

				default:
				return "";
			}
		}

	}
}
