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
	[Operation("postComment")]
	public class PostCommentOperation : JsonOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "targetModId", "commentBody" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
		public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
		{
			string targetModId = arguments["targetModId"];
			string commentBody = arguments["commentBody"];

			if (targetModId == null || commentBody == null)
			{
				return new PostCommentResponse()
				{
					Error = "All fields were not filled out"
				};
			}

			if (!authentication.IsSignedIn)
			{
				return new PostCommentResponse()
				{
					Error = "You are not signed in."
				};
			}

			if (!UploadedModsManager.Instance.HasModWithIdBeenUploaded(targetModId))
			{
				return new PostCommentResponse()
				{
					Error = "That mod does not exist"
				};
			}

			SpecialModData specialModData = UploadedModsManager.Instance.GetSpecialModInfoFromId(targetModId);

			string userId = authentication.UserID;

			string sanitized = commentBody.Replace("<", "&lt;").Replace(">", "&gt;");

			sanitized = sanitized.Replace("\n", "<br>");

			Comment comment = Comment.CreateNewComment(userId, sanitized);
			specialModData.Comments.Add(comment);
			specialModData.Save();

			return new PostCommentResponse()
			{
				message = "Comment posted."
			};
		}

		[Serializable]
		private class PostCommentResponse : JsonOperationResponseBase
		{
			public string message;
		}
	}
}
