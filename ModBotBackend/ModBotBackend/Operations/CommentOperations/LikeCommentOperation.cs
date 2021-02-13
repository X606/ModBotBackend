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
	[Operation("likeComment")]
	public class LikeCommentOperation : JsonOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "modId", "commentId", "likeState" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
		public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
		{
			string modId = arguments["modId"];
			string commentId = arguments["commentId"];
			bool likeState = arguments["likeState"];

			if (modId == null || commentId == null)
			{
				return new LikeCommentRequestResponse() {
					Error = "All fields were not filled out"
				};
			}

			if(!authentication.IsSignedIn)
			{
				return new LikeCommentRequestResponse()
				{
					Error = "You are not signed in."
				};
			}

			if(!UploadedModsManager.Instance.HasModWithIdBeenUploaded(modId))
			{
				return new LikeCommentRequestResponse()
				{
					Error = "No mod with that id exists"
				};
			}

			string userId = authentication.UserID;

			User user = UserManager.Instance.GetUserFromId(userId);
			SpecialModData modData = UploadedModsManager.Instance.GetSpecialModInfoFromId(modId);

			Comment comment = modData.GetCommentWithCommentID(commentId);

			if(likeState)
			{
				if(!comment.UsersWhoLikedThis.Contains(userId))
				{
					comment.UsersWhoLikedThis.Add(userId);
					modData.Save();
				}
				else
				{
					return new LikeCommentRequestResponse()
					{
						Error = "You have already liked that comment!"
					};
				}

			}
			else
			{
				if(comment.UsersWhoLikedThis.Contains(userId))
				{
					comment.UsersWhoLikedThis.Remove(userId);
					modData.Save();
				}
				else
				{
					return new LikeCommentRequestResponse()
					{
						Error = "You havent liked that comment."
					};
				}
			}

			return new LikeCommentRequestResponse()
			{
				message = "Your liked status has been updated!"
			};
		}

		[Serializable]
		private class LikeCommentRequestResponse : JsonOperationResponseBase
		{
			public string message;
		}
	}
}
