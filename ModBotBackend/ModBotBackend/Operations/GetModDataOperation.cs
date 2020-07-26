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
	public class GetModDataOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context)
		{
		 	string id = context.Request.QueryString["id"];

			context.Response.ContentType = "text/plain";
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(UploadedModsManager.GetModInfoJsonFromId(id));
			httpStream.Close();
		}

	}
}
