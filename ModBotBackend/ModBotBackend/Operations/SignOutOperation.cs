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
	[Operation("signOut")]
	public class SignOutOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			if (!authentication.IsSignedIn)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new SignOutResponse()
				{
					message = "You are not signed in.",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			SessionsManager.RemoveSession(authentication.SessionID);

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(new SignOutResponse()
			{
				message = "signed out!",
				isError = false
			}.ToJson());
			httpStream.Close();
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
