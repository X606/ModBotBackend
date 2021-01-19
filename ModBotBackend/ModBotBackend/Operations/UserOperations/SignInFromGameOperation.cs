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
	public class SignInFromGameOperation : OperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "username", "password", "playfabID" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

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

			
			User user = UserManager.Instance.GetUserFromId(session.OwnerUserID);
			if (user.PlayfabID != null && user.PlayfabID != request.playfabID)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new SignInResponse()
				{
					error = "The provided playfabID did not match the one associated with this account"
				}.ToJson());
				stream.Close();
				return;
			}
			else if(user.PlayfabID == null)
			{
				if (UserManager.Instance.GetUserFromPlayfabID(request.playfabID) != null)
				{
					Utils.Respond(context.Response, new SignInResponse()
					{
						error = "The provided playfabID is already associated with an account."
					});
					return;
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
			public string playfabID;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(playfabID);
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
