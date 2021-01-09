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
	[Operation("createAccout")]
	public class CreateAccountOperation : OperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "username", "password" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override string OverrideResolveJavascript =>
		@"if (e.isError == false) {
			setCurrentSessionId(e.sessionID);
		}

		resolve(e);";

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
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

			if (!User.IsValidUsername(request.username, out string usernameError))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new CreateAccountResponse()
				{
					error = string.Format("Invalid username, {0}", usernameError),
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}
			if(!User.IsValidPassword(request.password, out string passwordError))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new CreateAccountResponse()
				{
					error = string.Format("Invalid password, {0}", passwordError),
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			User newUser = User.CreateNewUser(request.username, request.password);

			Session session = UserManager.Instance.SignInAsUser(request.username, request.password);

			OutputConsole.WriteLine("New user signed up " + newUser.UserID + " (" + newUser.Username + ")");

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
