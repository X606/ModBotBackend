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
	public class GetImageOperation : OperationBase
	{
		
		public override void OnOperation(HttpListenerContext context)
		{
			string id = context.Request.QueryString["id"];

			if (!UploadedModsManager.HasModWithIdBeenUploaded(id))
			{
				Utils.RederectToErrorPage(context, "No mod with the id \"" + id + "\" has been uploaded");
				return;
			}

			ModInfo modInfo = UploadedModsManager.GetModInfoFromId(id);

			if (!modInfo.HasImage)
			{
				Utils.RederectToErrorPage(context, "The mod with the id \"" + id + "\" has no image");
				return;
			}


			string imageFilePath = UploadedModsManager.GetModPathFromID(id) + modInfo.ImageFileName;

			byte[] imageData = File.ReadAllBytes(imageFilePath);

			context.Response.ContentLength64 = imageData.LongLength;
			context.Response.OutputStream.Write(imageData, 0, imageData.Length);
			context.Response.Close();
		}

	}
}
