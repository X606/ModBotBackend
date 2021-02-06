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
		public override string OverrideAPICallJavascript => "let destroy = () => { element.src = \"/api/?operation=getModImage&size=\" + element.clientWidth + \"x\" + element.clientHeight + \"&id=\" + id; }; if(element.clientWidth == 0 || element.clientHeight == 0) { setTimeout(destroy,100); } else { destroy(); }";

		static ConcurrentDictionary<string, byte[]> _rescaledImageCache = new ConcurrentDictionary<string, byte[]>();

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string id = context.Request.QueryString["id"];

			if (!UploadedModsManager.Instance.HasModWithIdBeenUploaded(id))
			{
				ImageConverter converter = new ImageConverter();
				int statusCode = 0;
				byte[] data = WebsiteRequestProcessor.OnRequest("Assets/DefaultAvatar.png", out string contentType, ref statusCode);
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
			if (size == null)
			{
				size = "64x64";
			}

			if (!ImageResizer.TryScaleImageAndGetAsByteArray(context.Request.QueryString["id"], size, imageFilePath, _rescaledImageCache, out byte[] imgData))
            {
				ImageConverter converter = new ImageConverter();

				byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
				context.Response.ContentLength64 = data.LongLength;
				context.Response.OutputStream.Write(data, 0, data.Length);
				context.Response.Close();
				return;
            }

			

			context.Response.ContentType = "image/png";
			context.Response.ContentLength64 = imgData.LongLength;
			context.Response.OutputStream.Write(imgData, 0, imgData.Length);
			context.Response.Close();
		}

	}
}
