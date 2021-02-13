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
	[Operation("hasLikedComment")]
	public class HasLikedCommentOperation : PlainTextOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "modId", "commentId" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
		public override string OnOperation(Arguments arguments, Authentication authentication)
		{
			ContentType = "application/json";

			string modId = arguments["modId"];
			string commentId = arguments["commentId"];

			if(modId == null || commentId == null)
			{
				return "false";
			}

			if(!authentication.IsSignedIn)
			{
				return "false";
			}

			if(!UploadedModsManager.Instance.HasModWithIdBeenUploaded(modId))
			{
				return "false";
			}

			SpecialModData specialModData = UploadedModsManager.Instance.GetSpecialModInfoFromId(modId);

			Comment comment = specialModData.GetCommentWithCommentID(commentId);
			if (comment == null)
			{
				return "false";
			}

			string userId = authentication.UserID;

			bool hasLiked = comment.UsersWhoLikedThis.Contains(userId);

			return hasLiked ? "true" : "false";
		}

	}
}
