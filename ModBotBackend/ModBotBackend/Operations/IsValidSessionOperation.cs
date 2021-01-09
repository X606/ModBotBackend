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
	[Operation("isValidSession")]
	public class IsValidSessionOperation : OperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "sessionId" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			IsValidSessionRequest request = JsonConvert.DeserializeObject<IsValidSessionRequest>(json);

			if(!request.IsValidRequest())
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send("false");
				stream.Close();
				return;
			}

			if(!SessionsManager.VerifyKey(request.sessionId, out Session session))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send("false");
				stream.Close();
				return;
			} else
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send("true");
				stream.Close();
				return;
			}
		}

		[Serializable]
		private class IsValidSessionRequest
		{
			public string sessionId;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(sessionId);
			}
		}
	}
}
