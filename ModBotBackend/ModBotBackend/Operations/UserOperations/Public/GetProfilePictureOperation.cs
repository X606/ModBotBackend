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
using System.Collections.Concurrent;

namespace ModBotBackend.Operations
{
	[Operation("getProfilePicture")]
	public class GetProfilePictureOperation : OperationBase
	{
		public override bool ParseAsJson => false;
		public override string[] Arguments => new string[] { "element", "id" };
		public override bool ArgumentsInQuerystring => true;
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		//public override string OverrideAPICallJavascript => "element.src = \"/api/?operation=getProfilePicture&id=\" + id;";
		public override string OverrideAPICallJavascript => "let destroy = () => { element.src = \"/api/?operation=getProfilePicture&size=\" + element.clientWidth + \"x\" + element.clientHeight + \"&id=\" + id; }; if(element.clientWidth == 0 || element.clientHeight == 0) { setTimeout(destroy,100); } else { destroy(); }";

		static ConcurrentDictionary<string, byte[]> _rescaledImageCache = new ConcurrentDictionary<string, byte[]>();

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string id = context.Request.QueryString["id"];

			User user = UserManager.Instance.GetUserFromId(id);

			if(user == null)
			{
				ImageConverter converter = new ImageConverter();

				byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
				context.Response.ContentLength64 = data.LongLength;
				context.Response.OutputStream.Write(data, 0, data.Length);
				context.Response.Close();
				return;
			}

			string imageFilePath = UserManager.Instance.ProfilePicturesPath + id;

			if (File.Exists(imageFilePath + ".png"))
			{
				imageFilePath += ".png";
				context.Response.ContentType = "image/png";
			}
			else if(File.Exists(imageFilePath + ".jpg"))
			{
				imageFilePath += ".jpg";
				context.Response.ContentType = "image/jpeg";
			}
			else if(File.Exists(imageFilePath + ".gif"))
			{
				imageFilePath += ".gif";
				context.Response.ContentType = "image/gif";
			} else
			{
				imageFilePath = UserManager.Instance.ProfilePicturesPath + "DefaultAvatar.png";
			}

			string size = context.Request.QueryString["size"];
			if (size == null)
			{
				size = "32x32";
			}

            if (!ImageResizer.TryScaleImageAndGetAsByteArray(id, size, imageFilePath, _rescaledImageCache, out byte[] imageData))
			{
				ImageConverter converter = new ImageConverter();

				byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
				context.Response.ContentLength64 = data.LongLength;
				context.Response.OutputStream.Write(data, 0, data.Length);
				context.Response.Close();
				return;
			}

			context.Response.ContentType = "image/png";
			context.Response.ContentLength64 = imageData.LongLength;
			context.Response.OutputStream.Write(imageData, 0, imageData.Length);
			context.Response.Close();
		}

	}
}
