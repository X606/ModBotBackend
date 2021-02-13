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
	[Operation("signIn")]
	public class SignInOperation : JsonOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "username", "password" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override string OverrideResolveJavascript => 
		@"if (e.sessionID != null) {
			setCurrentSessionId(e.sessionID);
		}

		resolve(e);";

        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
		{
			ContentType = "text/plain";

			SignInData request = new SignInData()
			{
				password = arguments["password"],
				username = arguments["username"]
			};

			if(!request.IsValidRequest())
			{
				return new SignInResponse()
				{
					Error = "All fields were not filled out"
				};
			}
			
			Session session = UserManager.Instance.SignInAsUser(request.username, request.password);

			if (session == null)
			{
				return new SignInResponse()
				{
					Error = "Either the specified username or the password was wrong"
				};
			}

			OutputConsole.WriteLine(request.username + " signed in");

			return new SignInResponse()
			{
				sessionID = session.Key
			};
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
		private class SignInResponse : JsonOperationResponseBase
		{
			public string sessionID;
		}
	}
}
