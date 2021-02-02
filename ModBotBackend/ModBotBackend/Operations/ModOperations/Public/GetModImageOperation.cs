using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using System.IO;
using ModBotBackend.Users;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Concurrent;

namespace ModBotBackend.Operations
{
	
	[Operation("getModImage")]
	public class GetModImageOperation : OperationBase
	{
		public override bool ParseAsJson => false;
		public override string[] Arguments => new string[] { "element", "id"};
		public override bool ArgumentsInQuerystring => true;
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
		public override string OverrideAPICallJavascript => "element.src = \"/api/?operation=getModImage&size=\" + element.clientWidth + \"x\" + element.clientHeight + \"&id=\" + id;";

		static ConcurrentDictionary<string, byte[]> _rescaledImageCache = new ConcurrentDictionary<string, byte[]>();

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string id = context.Request.QueryString["id"];

			if (!UploadedModsManager.Instance.HasModWithIdBeenUploaded(id))
			{
				ImageConverter converter = new ImageConverter();

				byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
				context.Response.ContentLength64 = data.LongLength;
				context.Response.OutputStream.Write(data, 0, data.Length);
				context.Response.Close();
				return;
			}

			ModInfo modInfo = UploadedModsManager.Instance.GetModInfoFromId(id);

			if (!modInfo.HasImage)
			{
				ImageConverter converter = new ImageConverter();

				byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
				context.Response.ContentLength64 = data.LongLength;
				context.Response.OutputStream.Write(data, 0, data.Length);
				context.Response.Close();
				return;
			}


			string imageFilePath = UploadedModsManager.Instance.GetModPathFromID(id) + modInfo.ImageFileName;

			string size = context.Request.QueryString["size"];
			string[] split = size.Split('x');

			if (split.Length != 2)
			{
				ImageConverter converter = new ImageConverter();

				byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
				context.Response.ContentLength64 = data.LongLength;
				context.Response.OutputStream.Write(data, 0, data.Length);
				context.Response.Close();

				return;
			}

			int[] dimentions = new int[split.Length];
			for (int i = 0; i < dimentions.Length; i++)
			{
				if (int.TryParse(split[i], out int result))
				{
					dimentions[i] = result;
				}
				else
				{
					ImageConverter converter = new ImageConverter();

					byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
					context.Response.ContentLength64 = data.LongLength;
					context.Response.OutputStream.Write(data, 0, data.Length);
					context.Response.Close();
					return;
				}
			}

			if (Math.Max(dimentions[0],dimentions[1]) > 512 || Math.Min(dimentions[0], dimentions[1]) < 1)
			{
				ImageConverter converter = new ImageConverter();

				byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
				context.Response.ContentLength64 = data.LongLength;
				context.Response.OutputStream.Write(data, 0, data.Length);
				context.Response.Close();
				return;
			}

			byte[] imageData;
			if (_rescaledImageCache.TryGetValue(context.Request.QueryString["id"] + context.Request.QueryString["size"], out byte[] imgData))
			{
				imageData = imgData;
			}
			else
			{
				using (var ms = new FileStream(imageFilePath, FileMode.Open))
				{
					using (Bitmap bmp = new Bitmap(ms))
					{

						float ratio = ((float)dimentions[0]) / ((float)dimentions[1]);

						int width;
						int height;

						width = (int)(bmp.Height * ratio);
						height = bmp.Height;

						using (Bitmap cropped = bmp.CropImageAtRect(new Rectangle((bmp.Width - width) / 2, (bmp.Height - height) / 2, width, height)))
						{
							using (Bitmap resized = Utils.ResizeImage(cropped, dimentions[0], dimentions[1]))
							{

								using (var saveStream = new MemoryStream())
								{
									resized.Save(saveStream, ImageFormat.Png);

									imageData = saveStream.ToArray();
								}
							}
						}
					}
				}
				_rescaledImageCache.TryAdd(context.Request.QueryString["id"] + context.Request.QueryString["size"], imageData);
			}

			context.Response.ContentType = "image/png";
			context.Response.ContentLength64 = imageData.LongLength;
			context.Response.OutputStream.Write(imageData, 0, imageData.Length);
			context.Response.Close();
		}

	}
}
