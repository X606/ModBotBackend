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

namespace ModBotBackend.Operations
{
	public class SignOutOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			SignOutData request = Newtonsoft.Json.JsonConvert.DeserializeObject<SignOutData>(json);

			if(!request.IsValidRequest())
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new SignOutResponse()
				{
					message = "All fields were not filled out",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			if (!SessionsManager.VerifyKey(request.sessionID, out Session session))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new SignOutResponse()
				{
					message = "The provided session id was invalid or outdated",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			SessionsManager.RemoveSession(request.sessionID);

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(new SignOutResponse()
			{
				message = "signed out!",
				isError = false
			}.ToJson());
			httpStream.Close();
		}

		[Serializable]
		private class SignOutData
		{
			public string sessionID;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(sessionID);
			}
		}
		[Serializable]
		private class SignOutResponse
		{
			public string message;
			public bool isError;

			public string ToJson()
			{
				return Newtonsoft.Json.JsonConvert.SerializeObject(this);
			}
		}
	}
}
