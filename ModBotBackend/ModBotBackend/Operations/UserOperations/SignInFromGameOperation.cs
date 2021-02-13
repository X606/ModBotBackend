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
	[Operation("signInFromGame")]
	public class SignInFromGameOperation : JsonOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "username", "password", "playfabID" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
		{
			SignInData request = new SignInData()
			{
				password = arguments["password"],
				playfabID = arguments["playfabID"],
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

			User user = UserManager.Instance.GetUserFromId(session.OwnerUserID);
			if (user.PlayfabID != null && user.PlayfabID != request.playfabID)
			{
				return new SignInResponse() { Error = "The provided playfabID did not match the one associated with this account" };
            }
			else if(user.PlayfabID == null)
			{
				if (UserManager.Instance.GetUserFromPlayfabID(request.playfabID) != null)
				{
					return new SignInResponse()
					{
						Error = "The provided playfabID is already associated with an account."
					};
				}

				user.PlayfabID = request.playfabID;
				user.Save();
			}

			if (user.AuthenticationLevel < AuthenticationLevel.VerifiedUser)
			{
				Console.WriteLine("Verified user: " + user.Username);
				user.AuthenticationLevel = AuthenticationLevel.VerifiedUser;
				user.Save();
			}

			OutputConsole.WriteLine(request.username + " signed in through game");

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
			public string playfabID;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(playfabID);
			}
		}
		[Serializable]
		private class SignInResponse : JsonOperationResponseBase
		{
			public string sessionID;
		}
	}
}
