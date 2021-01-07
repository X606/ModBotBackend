using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using System.IO;

namespace ModBotBackend.Operations
{
	
	[Operation("getModImage")]
	public class GetImageOperation : OperationBase
	{
		
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string id = context.Request.QueryString["id"];

			if (!UploadedModsManager.Instance.HasModWithIdBeenUploaded(id))
			{
				Utils.RederectToErrorPage(context, "No mod with the id \"" + id + "\" has been uploaded");
				return;
			}

			ModInfo modInfo = UploadedModsManager.Instance.GetModInfoFromId(id);

			if (!modInfo.HasImage)
			{
				Utils.RederectToErrorPage(context, "The mod with the id \"" + id + "\" has no image");
				return;
			}


			string imageFilePath = UploadedModsManager.Instance.GetModPathFromID(id) + modInfo.ImageFileName;

			string[] imageFilePathSplit = imageFilePath.Split('.');
			string format = imageFilePathSplit[imageFilePathSplit.Length-1].ToLower();
			if (format == "png")
			{
				context.Response.ContentType = "image/png";
			}
			else if(format == "jpg")
			{
				context.Response.ContentType = "image/jpeg";
			}
			else if(format == "gif")
			{
				context.Response.ContentType = "image/gif";
			}

			byte[] imageData = File.ReadAllBytes(imageFilePath);

			
			context.Response.ContentLength64 = imageData.LongLength;
			context.Response.OutputStream.Write(imageData, 0, imageData.Length);
			context.Response.Close();
		}

	}
}
