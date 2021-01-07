using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Security.Cryptography;

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

		public static void ClearFileCache()
		{
			_fileExistsCache.Clear();
			_cachedFileData.Clear();
		}

		static Dictionary<string, bool> _fileExistsCache = new Dictionary<string, bool>();
		public static bool FileExistsCached(string path)
		{
#if !DEBUG
			if (_fileExistsCache.TryGetValue(path, out bool fileExits))
			{
				return fileExits;
			}

			fileExits = File.Exists(path);
			_fileExistsCache.Add(path, fileExits);
			return fileExits;
#else
			return File.Exists(path);
#endif
		}
		static Dictionary<string, byte[]> _cachedFileData = new Dictionary<string, byte[]>();
		public static byte[] FileReadAllBytesCached(string path)
		{
#if !DEBUG
			if(_cachedFileData.TryGetValue(path, out byte[] fileData))
			{
				return fileData;
			}

			fileData = File.ReadAllBytes(path);
			_cachedFileData.Add(path, fileData);
			return fileData;
#else
			return File.ReadAllBytes(path);
#endif
		}

		public static string FileReadAllTextCached(string path)
		{
			byte[] data = FileReadAllBytesCached(path);
			return Encoding.UTF8.GetString(data);
		}

		const string CHARACTERS_TO_USE_IN_KEYS = "abcdefghijklmopqrstuvwxyz";

		public static string GenerateSecureKey()
		{
			byte[] buffer = new byte[16];
			new RNGCryptoServiceProvider().GetBytes(buffer);
			string generatedKey = "";
			for(int i = 0; i < 16; i++)
			{
				int index = buffer[i] % CHARACTERS_TO_USE_IN_KEYS.Length;
				generatedKey += CHARACTERS_TO_USE_IN_KEYS[index];
			}

			return generatedKey;
		}

		
		public static string FormatString(string str, params string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				string searchString = "{" + i + "}";
				int[] indecies = GetIndexesInString(str, searchString);

				for (int j = indecies.Length - 1; j >= 0; j--)
				{
					int index = indecies[j];
					string pre = str.Substring(0, index);
					string post = str.Substring(index + searchString.Length);

					str = pre + args[i] + post;
				}

			}

			return str;
		}

		public static int[] GetIndexesInString(string str, string substring)
		{
			List<int> indexies = new List<int>();

			for (int i = 0; i < str.Length; i++)
			{
				for (int j = 0; j < substring.Length; j++)
				{
					if ((i + j) == str.Length)
						break;

					if (str[i + j] != substring[j])
						break;

					if (j == (substring.Length - 1))
					{
						indexies.Add(i);
					}
				}


			}

			return indexies.ToArray();
		}

	}
}
