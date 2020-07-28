using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HttpUtils;
using ModLibrary;
using ModBotBackend.Users;
using ModBotBackend.Users.Sessions;
using Newtonsoft.Json;

namespace ModBotBackend.Operations
{
	public class GetCurrentUserOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			GetCurrentUserOperationRequest request = JsonConvert.DeserializeObject<GetCurrentUserOperationRequest>(json);

			if(!request.IsValidRequest())
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send("null");
				stream.Close();
				return;
			}

			if(!SessionsManager.VerifyKey(request.sessionId, out Session session))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send("null");
				stream.Close();
				return;
			}
			
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(session.OwnerUserID);
			httpStream.Close();
		}

		[Serializable]
		private class GetCurrentUserOperationRequest
		{
			public string sessionId;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(sessionId);
			}
		}
	}
}
