using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using System.IO;
using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
	[Operation("getProfilePicture")]
	public class GetProfilePictureOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string id = context.Request.QueryString["id"];

			User user = UserManager.Instance.GetUserFromId(id);

			if(user == null)
			{
				Utils.RederectToErrorPage(context, "No user with the id \"" + id + "\" has been found");
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
			
			byte[] imageData = File.ReadAllBytes(imageFilePath);
			
			context.Response.ContentLength64 = imageData.LongLength;
			context.Response.OutputStream.Write(imageData, 0, imageData.Length);
			context.Response.Close();
		}

	}
}
