using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HttpUtils;
using Newtonsoft.Json;
using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
	[Operation("getAllModIds")]
	public class GetAllModIdsOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string[] ids = UploadedModsManager.Instance.GetAllUploadedIds();

			string json = JsonConvert.SerializeObject(ids);

			context.Response.ContentType = "text/plain";
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(json);
			httpStream.Close();
		}

	}
}
