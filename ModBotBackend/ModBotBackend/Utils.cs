using HttpUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace ModBotBackend
{
    public static class Utils
    {
        public static int GetMaxPlayerTags(Authentication auth)
        {
            return 3;
        }
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

        public static void SendErrorPage(HttpListenerResponse response, string message, bool isError, HttpStatusCode status)
        {
            int statusCode = -1;
            string html = Encoding.UTF8.GetString(WebsiteRequestProcessor.OnRequest("/errorPage.html", out string contentType, ref statusCode));

            response.StatusCode = (int)status;
            response.ContentType = "text/html";
            html = Utils.FormatString(html, message, isError ? "" : "display: none;", isError ? "display: none;" : "");

            HttpStream stream = new HttpStream(response);
            stream.Send(html);
            stream.Close();
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
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
        public static void RecursivelyDeleteFolder(string folder)
        {
            string[] folders = Directory.GetDirectories(folder);
            foreach (string subFolder in folders)
            {
                RecursivelyDeleteFolder(subFolder);
            }

            string[] files = Directory.GetFiles(folder);
            foreach (string file in files)
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
            for (int i = 0; i < 16; i++)
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

        public static bool TryGetRequestBody<T>(HttpListenerContext context, out T _out) where T : class
        {
            try
            {
                byte[] data = Misc.ToByteArray(context.Request.InputStream);
                string json = Encoding.UTF8.GetString(data);

                _out = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch
            {
                _out = null;
                return false;
            }
        }

        public static void Respond(HttpListenerResponse response, string body)
        {
            HttpStream stream = new HttpStream(response);
            stream.Send(body);
            stream.Close();
        }
        public static void Respond<T>(HttpListenerResponse response, T body)
        {
            Respond(response, JsonConvert.SerializeObject(body));
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height, InterpolationMode interpolationMode)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = interpolationMode;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Bitmap CropImageAtRect(this Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }
    }

    public static class BitCompressor
    {
        public static byte[] GetBytes<T>(T obj)
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            Array.Sort(fields, (x, y) => String.Compare(x.Name, y.Name));

            byte[] data;

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, obj);

                data = stream.ToArray();
            }

            return data;
        }
        public static void FillObj<T>(T obj, byte[] data)
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            Array.Sort(fields, (x, y) => String.Compare(x.Name, y.Name));

            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                T t = (T)bf.Deserialize(stream);

                for (int i = 0; i < fields.Length; i++)
                {
                    fields[i].SetValue(obj, fields[i].GetValue(t));
                }

            }

        }

    }
}
