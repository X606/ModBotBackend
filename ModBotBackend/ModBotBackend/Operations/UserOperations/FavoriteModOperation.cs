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
	[Operation("favoriteMod")]
	public class FavoriteModOperation : JsonOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "modID", "favorite" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
		{
			Request request = new Request()
			{
				favorite = arguments["favorite"],
				modID = arguments["modID"]
			};

			if (!request.IsValidRequest())
			{
				return new Response()
				{
					Error = "The request wasn't valid."
				};
			}
			if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.BasicUser))
			{
				return new Response()
				{
					Error = "You are not signed in"
				};
			}
			if (!UploadedModsManager.Instance.HasModWithIdBeenUploaded(request.modID))
			{
				return new Response()
				{
					Error = "No mod with that id has been uploaded."
				};
			}

			User user = UserManager.Instance.GetUserFromId(authentication.UserID);
			bool isFavorited = user.FavoritedMods.Contains(request.modID);
			
			if ((isFavorited && request.favorite) || (!isFavorited && !request.favorite))
			{
				return new Response()
				{
					message = "favorited status not changed"
				};
			}

			if (request.favorite)
			{
				user.FavoritedMods.Add(request.modID);
			} else
			{
				user.FavoritedMods.Remove(request.modID);
			}

			return new Response()
			{
				message = "Favorited status updated"
			};
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
		class Response : JsonOperationResponseBase
		{
			public string message;
		}

	}
}
