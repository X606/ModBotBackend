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
	[Operation("getModData")]
	public class GetModDataOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
		 	string id = context.Request.QueryString["id"];

			context.Response.ContentType = "text/plain";
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(UploadedModsManager.Instance.GetModInfoJsonFromId(id));
			httpStream.Close();
		}

	}
}
