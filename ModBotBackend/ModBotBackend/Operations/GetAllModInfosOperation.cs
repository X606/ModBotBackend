using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HttpUtils;
using Newtonsoft.Json;
using ModLibrary;

namespace ModBotBackend.Operations
{
	public class GetAllModInfosOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string[] ids = UploadedModsManager.GetAllUploadedIds();

			ModInfo[] modInfos = new ModInfo[ids.Length];
			for(int i = 0; i < ids.Length; i++)
			{
				modInfos[i] = UploadedModsManager.GetModInfoFromId(ids[i]);
			}

			string json = JsonConvert.SerializeObject(modInfos);

			context.Response.ContentType = "text/plain";
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(json);
			httpStream.Close();
		}

	}
}
