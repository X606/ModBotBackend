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
	[Operation("deleteComment")]
	public class DeleteCommentOperation : JsonOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "targetModId", "targetCommentId" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;

		public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
		{
			DeleteCommentRequest request = new DeleteCommentRequest()
			{
				targetCommentId = arguments["targetCommentId"],
				targetModId = arguments["targetModId"]
			};

			if(!request.IsValidRequest())
			{
				return new DeleteCommentResponse()
				{
					Error = "All fields were not filled out"
				};
			}

			if(!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.BasicUser))
			{
				return new DeleteCommentResponse()
				{
					Error = "You are not signed in"
				};
			}

			if(!UploadedModsManager.Instance.HasModWithIdBeenUploaded(request.targetModId))
			{
				return new DeleteCommentResponse()
				{
					Error = "That mod does not exist"
				};
			}

			SpecialModData specialModData = UploadedModsManager.Instance.GetSpecialModInfoFromId(request.targetModId);

			string userId = authentication.UserID;

			Comment comment = specialModData.GetCommentWithCommentID(request.targetCommentId);
			if (comment == null)
			{
				return new DeleteCommentResponse()
				{
					Error = "There is no comment with that id on that mod"
				};
			}

			if (userId != comment.PosterUserId && !authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.Admin))
			{
				return new DeleteCommentResponse()
				{
					Error = "You do not have premission to delete this comment"
				};
			}

			specialModData.DeleteCommentWithId(request.targetCommentId);
			specialModData.Save();

			return new DeleteCommentResponse()
			{
				message = "Comment deleted."
			};
		}

		[Serializable]
		private class DeleteCommentRequest
		{
			public string targetModId;
			public string targetCommentId;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(targetModId);
			}
		}
		[Serializable]
		private class DeleteCommentResponse : JsonOperationResponseBase
		{
			public string message;

			public string ToJson()
			{
				return JsonConvert.SerializeObject(this);
			}
		}
	}
}
