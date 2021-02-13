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
using Newtonsoft.Json;

namespace ModBotBackend.Operations
{
	[Operation("createAccount")]
	public class CreateAccountOperation : JsonOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "username", "password" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override string OverrideResolveJavascript =>
		@"if (e.isError == false) {
			setCurrentSessionId(e.sessionID);
		}

		resolve(e);";

		public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
		{
			ContentType = "text/plain";

			CreateAccountData request = new CreateAccountData()
			{
				username = arguments["username"],
				password = arguments["password"]
			};

			if(!request.IsValidRequest())
			{
				return new CreateAccountResponse()
				{
					Error = "All fields were not filled out"
				};
			}

			if (!User.IsValidUsername(request.username, out string usernameError))
			{
				return new CreateAccountResponse()
				{
					Error = string.Format("Invalid username, {0}", usernameError)
				};
			}
			if(!User.IsValidPassword(request.password, out string passwordError))
			{
				return new CreateAccountResponse()
				{
					Error = string.Format("Invalid password, {0}", passwordError)
				};
			}

			User newUser = User.CreateNewUser(request.username, request.password);

			Session session = UserManager.Instance.SignInAsUser(request.username, request.password);

			OutputConsole.WriteLine("New user signed up " + newUser.UserID + " (" + newUser.Username + ")");

			return new CreateAccountResponse()
			{
				sessionID = session.Key
			};
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
		private class CreateAccountResponse : JsonOperationResponseBase
		{
			public string sessionID;
		}
	}
}
