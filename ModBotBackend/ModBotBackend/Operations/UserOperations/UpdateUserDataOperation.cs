using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend;
using ModBotBackend.Operations;
using ModBotBackend.Users;
using ModBotBackend.Users.Sessions;
using Newtonsoft.Json;
using HttpUtils;

namespace ModBotBackend.Operations
{
	[Operation("updateUserData")]
	public class UpdateUserDataOperation : JsonOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "password", "username", "bio", "newPassword", "borderStyle", "showFull" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
		public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
		{
			Request request = new Request()
			{
				bio = arguments["bio"],
				borderStyle = arguments["borderStyle"] != null ? (BorderStyles?)(int)arguments["borderStyle"] : null,
				newPassword = arguments["newPassword"],
				password = arguments["password"],
				showFull = arguments["showFull"],
				username = arguments["username"]
			};

			if (!request.IsValidRequest())
			{
				return new Response(true, "The request wasn't valid");
			}

			if (!authentication.IsSignedIn)
			{
				return new Response(true, "The provided session id was invalid or outdated");
			}

			User user = UserManager.Instance.GetUserFromId(authentication.UserID);

			if (!user.VeryfyPassword(request.password))
			{
				return new Response(true, "The provided password was wrong");
			}

			if(user.Username == request.username)
				request.username = null;
			if(user.Bio == request.bio)
				request.bio = null;
			if(user.BorderStyle == request.borderStyle)
				request.borderStyle = null;
            /*if (user.ShowFull == request.showFull)
                request.showFull = null;*/

            if (request.username != null)
			{
				if(!User.IsValidUsername(request.username, out string error))
				{
					return new Response(true, error);
				}
				user.Username = request.username;
			}
			if (request.bio != null)
			{
				user.Bio = System.Web.HttpUtility.HtmlEncode(request.bio);
			}
			if (request.newPassword != null)
			{
				if(!User.IsValidPassword(request.newPassword, out string error))
				{
					return new Response(true, error);
				}
				user.SetPassword(request.newPassword);
			}
			if (request.borderStyle != null)
			{
				user.BorderStyle = request.borderStyle.Value;
			}
            if (request.showFull != null)
            {
                user.ShowFull = request.showFull.Value;
            }

			user.Save();

			OutputConsole.WriteLine(user.UserID + " (" + user.Username + ") updated their profile.");

			return new Response();
		}

		public class Request
		{
			public string username;
			public string bio;
			public string newPassword;
			public BorderStyles? borderStyle;
            public bool? showFull;

			public string password;

			public bool IsValidRequest()
			{
				return password != null;
			}
		}
		public class Response : JsonOperationResponseBase
		{
			
			public bool isError;
			public string message;

			public Response(bool isError = false, string message = "")
			{
				if (isError)
                {
					Error = message;
				}
				else
                {
					this.message = message;
				}
			}

			public string ToJson()
			{
				return JsonConvert.SerializeObject(this);
			}
		}

	}
}
