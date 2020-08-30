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

			if (!isValidUsername(request.username, out string usernameError))
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
			if(!isValidPassword(request.password, out string passwordError))
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

		const int MIN_USERNAME_LENGTH = 3;
		const int MAX_USERNAME_LENGTH = 40;

		const string ALLOWED_CHARACTERS_IN_USERNAMES = "abcdefghijklmnopqrstuvwxyzåäöABCDEFGIJKLMNOPQRSTUVWXYZÅÄÖ1234567890";

		bool isValidUsername(string username, out string error)
		{
			if (username.Length < MIN_USERNAME_LENGTH)
			{
				error = "invalid length, the username must be at least " + MIN_USERNAME_LENGTH + " characters long";
				return false;
			}
			if(username.Length > MAX_USERNAME_LENGTH)
			{
				error = "invalid length, the username cannot be longer than " + MAX_USERNAME_LENGTH + " characters";
				return false;
			}

			for(int i = 0; i < username.Length; i++)
			{
				if(!ALLOWED_CHARACTERS_IN_USERNAMES.Contains(username[i]))
				{
					error = "the character \"" + username[i] + "\" is not allowed in usernames";
					return false;
				}
			}

			error = string.Empty;
			return true;
		}
		bool isValidPassword(string password, out string error)
		{
			if(password.Length < 4)
			{
				error = "invalid length, the password must be at least 4 characters long";
				return false;
			}
			if(password.Length > 100)
			{
				error = "invalid length, the password is too long, the max limit is 100 characters";
				return false;
			}

			error = string.Empty;
			return true;
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
