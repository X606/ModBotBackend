using ModBotBackend.Managers;
using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.TagsOperations
{
	[Operation("setPlayerTags")]
	public class SetPlayerTags : OperationBase
	{
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
		public override string[] Arguments => new string[] { "tags" };
		public override bool ParseAsJson => true;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			if (!Utils.TryGetRequestBody(context, out Request request))
			{
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "Invalid request"
				});
				return;
			}
			if (!authentication.IsSignedIn)
			{
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "Not signed in"
				});
				return;
			}
			if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.VerifiedUser))
			{
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "You need to be verified to set your player tags"
				});
				return;
			}
			for (int i = 0; i < request.tags.Length; i++)
			{
				TagInfo tag = TagsManager.Instance.GetTag(request.tags[i]);
				if (!tag.Verified)
				{
					Utils.Respond(context.Response, new Response()
					{
						isError = true,
						message = "The tag \"" + tag.TagID + "\" is not verified, so you cant use it yet."
					});
					return;
				}
			}


			User user = UserManager.Instance.GetUserFromId(authentication.UserID);

			TagsManager.Instance.SaveUserTags(user.PlayfabID, new PlayerTagsInfo(request.tags));

			Utils.Respond(context.Response, new Response()
			{
				isError = false,
				message = "Updated tags for player with playfab id \"" + user.PlayfabID + "\""
			});
		}

		class Request
		{
			public string[] tags;
		}

		class Response
		{
			public bool isError;
			public string message;
		}

	}
}
