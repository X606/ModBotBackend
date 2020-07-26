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
	public class CreateAccountOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);
			
			CreateAccountData request = JsonConvert.DeserializeObject<CreateAccountData>(json);

			if(!request.IsValidRequest())
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new CreateAccountResponse()
				{
					error = "All fields were not filled out",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			if (UserManager.GetUserFromUsername(request.username) != null)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new CreateAccountResponse()
				{
					error = "That username is already taken",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			User newUser = User.CreateNewUser(request.username, request.password);

			Session session = UserManager.SignInAsUser(request.username, request.password);
			
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(new CreateAccountResponse()
			{
				sessionID = session.Key,
				isError = false
			}.ToJson());
			httpStream.Close();
		}

		[Serializable]
		private class CreateAccountData
		{
			public string username;
			public string password;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
			}
		}
		[Serializable]
		private class CreateAccountResponse
		{
			public string sessionID;
			public string error;
			public bool isError;

			public string ToJson()
			{
				return JsonConvert.SerializeObject(this);
			}
		}
	}
}
