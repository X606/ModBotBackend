using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HttpUtils;

namespace ModBotBackend.Operations
{
	[Operation("downloadMod")]
	public class DownloadModOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string id = context.Request.QueryString["id"];

			if(!UploadedModsManager.HasModWithIdBeenUploaded(id))
			{
				Utils.RederectToErrorPage(context, "No mod with the id \"" + id + "\" has been uploaded");
				return;
			}

			string path = UploadedModsManager.GetZippedModPathFromID(id);

			byte[] data = File.ReadAllBytes(path);

			string displayName = UploadedModsManager.GetModInfoFromId(id).DisplayName + ".zip";
			displayName = displayName.Replace(' ', '_');
			foreach(char c in Path.GetInvalidFileNameChars())
			{
				displayName = displayName.Replace(c, '_');
			}

			SpecialModData specialModData = UploadedModsManager.GetSpecialModInfoFromId(id);
			specialModData.Downloads++;
			specialModData.Save();

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.SendFile(data, displayName);
			httpStream.Close();
		}

	}
}
