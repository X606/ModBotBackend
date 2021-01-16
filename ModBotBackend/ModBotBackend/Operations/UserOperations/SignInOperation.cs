﻿using System;
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
	[Operation("signIn")]
	public class SignInOperation : OperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "username", "password" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override string OverrideResolveJavascript => 
		@"if (e.sessionID != null) {
			setCurrentSessionId(e.sessionID);
		}

		resolve(e);";

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			SignInData request = Newtonsoft.Json.JsonConvert.DeserializeObject<SignInData>(json);

			if(!request.IsValidRequest())
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new SignInResponse()
				{
					error = "All fields were not filled out"
				}.ToJson());
				stream.Close();
				return;
			}
			
			Session session = UserManager.Instance.SignInAsUser(request.username, request.password);

			if (session == null)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new SignInResponse()
				{
					error = "Either the specified username or the password was wrong"
				}.ToJson());
				stream.Close();
				return;
			}

			OutputConsole.WriteLine(request.username + " signed in");

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(new SignInResponse()
			{
				sessionID = session.Key
			}.ToJson());
			httpStream.Close();
		}

		[Serializable]
		private class SignInData
		{
			public string username;
			public string password;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
			}
		}
		[Serializable]
		private class SignInResponse
		{
			public string sessionID;
			public string error;

			public string ToJson()
			{
				return Newtonsoft.Json.JsonConvert.SerializeObject(this);
			}
		}
	}
}