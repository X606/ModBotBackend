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
	[Operation("getMyTags")]
	public class GetMyTagsOperation : OperationBase
	{
		public override string[] Arguments => new string[] { };

		public override bool ParseAsJson => true;

		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.VerifiedUser))
			{
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				Utils.Respond(context.Response, new Result()
				{
					isError = true
				});
				return;
			}

			User user = UserManager.Instance.GetUserFromId(authentication.UserID);

			TagInfo[] tags = TagsManager.Instance.GetTagsForPlayfabId(user.PlayfabID);

			string[] tagIds = new string[tags.Length];
			for (int i = 0; i < tags.Length; i++)
			{
				tagIds[i] = tags[i].TagID;
			}

			Utils.Respond(context.Response, new Result()
			{
				isError = false,
				Tags = tagIds
			});
		}


		class Result
		{
			public bool isError;
			public string[] Tags;
		}
	}
}
