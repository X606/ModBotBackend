using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend;
using ModBotBackend.Users;
using ModBotBackend.Users.Sessions;
using HttpUtils;
using Newtonsoft.Json;

namespace ModBotBackend.Operations
{
	public class FavoriteModOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "application/json";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			Request request = JsonConvert.DeserializeObject<Request>(json);

			if (!request.IsValidRequest())
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new Response("The request wasn't valid.").ToJson());
				stream.Close();
				return;
			}
			if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.BasicUser))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new Response("You are not signed in").ToJson());
				stream.Close();
				return;
			}
			if (!UploadedModsManager.HasModWithIdBeenUploaded(request.modID))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new Response("No mod with that id has been uploaded.").ToJson());
				stream.Close();
				return;
			}

			User user = UserManager.GetUserFromId(authentication.UserID);
			bool isFavorited = user.FavoritedMods.Contains(request.modID);
			
			if ((isFavorited && request.favorite) || (!isFavorited && !request.favorite))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new Response(false, "favorited status not changed").ToJson());
				stream.Close();
				return;
			}

			if (request.favorite)
			{
				user.FavoritedMods.Add(request.modID);
			} else
			{
				user.FavoritedMods.Remove(request.modID);
			}

			HttpStream resultStream = new HttpStream(context.Response);
			resultStream.Send(new Response(false, "Favorited status updated").ToJson());
			resultStream.Close();
		}

		class Request
		{
			public string modID;
			public bool favorite;

			public bool IsValidRequest()
			{
				return true;
			}
		}
		class Response
		{
			public Response(string errorMessage = null)
			{
				if (errorMessage != null)
				{
					isError = true;
					message = errorMessage;
				} else
				{
					isError = false;
					message = "";
				}
			}
			public Response(bool isError, string message)
			{
				this.isError = isError;
				this.message = message;
			}

			public bool isError;
			public string message;

			public string ToJson()
			{
				return JsonConvert.SerializeObject(this);
			}
		}

	}
}
