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
	public class UpdateUserDataOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "application/json";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			Request request = JsonConvert.DeserializeObject<Request>(json);

			if (!request.IsValidRequest())
			{
				HttpStream invalidRequestStream = new HttpStream(context.Response);
				invalidRequestStream.Send(new Response(true, "The request wasn't valid").ToJson());
				invalidRequestStream.Close();

				return;
			}

			if (!authentication.IsSignedIn)
			{
				HttpStream invalidRequestStream = new HttpStream(context.Response);
				invalidRequestStream.Send(new Response(true, "The provided session id was invalid or outdated").ToJson());
				invalidRequestStream.Close();

				return;
			}

			User user = UserManager.GetUserFromId(authentication.UserID);

			if (!user.VeryfyPassword(request.password))
			{
				HttpStream invalidRequestStream = new HttpStream(context.Response);
				invalidRequestStream.Send(new Response(true, "The provided password was wrong").ToJson());
				invalidRequestStream.Close();

				return;
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
					HttpStream invalidRequestStream = new HttpStream(context.Response);
					invalidRequestStream.Send(new Response(true, error).ToJson());
					invalidRequestStream.Close();

					return;
				}
				user.Username = request.username;
			}
			if (request.bio != null)
			{
				user.Bio = request.bio;
			}
			if (request.newPassword != null)
			{
				if(!User.IsValidPassword(request.newPassword, out string error))
				{
					HttpStream invalidRequestStream = new HttpStream(context.Response);
					invalidRequestStream.Send(new Response(true, error).ToJson());
					invalidRequestStream.Close();

					return;
				}
				user.SetPassword(request.newPassword);
			}
			if (request.borderStyle != null)
			{
				user.BorderStyle = request.borderStyle.Value;
			}
            if (request.showFull != null)
            {
                user.ShowFull = request.showFull;
            }

			user.Save();

            HttpStream resopnseStream = new HttpStream(context.Response);
			resopnseStream.Send(new Response().ToJson());
			resopnseStream.Close();
		}

		public class Request
		{
			public string username;
			public string bio;
			public string newPassword;
			public BorderStyles? borderStyle;
            public bool showFull;

			public string password;

			public bool IsValidRequest()
			{
				return password != null;
			}
		}
		public class Response
		{
			
			public bool isError;
			public string message;

			public Response(bool isError = false, string message = "")
			{
				this.isError=isError;
				this.message=message;
			}

			public string ToJson()
			{
				return JsonConvert.SerializeObject(this);
			}
		}

	}
}
