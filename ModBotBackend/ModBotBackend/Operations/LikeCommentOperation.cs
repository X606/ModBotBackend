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
	public class LikeCommentOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			LikeCommentRequestData request = JsonConvert.DeserializeObject<LikeCommentRequestData>(json);

			if(!request.IsValidRequest())
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new LikeCommentRequestResponse()
				{
					message = "All fields were not filled out",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			if(!authentication.IsSignedIn)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new LikeCommentRequestResponse()
				{
					message = "You are not signed in.",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			if(!UploadedModsManager.HasModWithIdBeenUploaded(request.modId))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new LikeCommentRequestResponse()
				{
					message = "No mod with that id exists",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			string userId = authentication.UserID;

			User user = UserManager.GetUserFromId(userId);
			SpecialModData modData = UploadedModsManager.GetSpecialModInfoFromId(request.modId);

			Comment comment = modData.GetCommentWithCommentID(request.commentId);

			if(request.likeState)
			{
				if(!comment.UsersWhoLikedThis.Contains(userId))
				{
					comment.UsersWhoLikedThis.Add(userId);
					modData.Save();
				}
				else
				{
					HttpStream stream = new HttpStream(context.Response);
					stream.Send(new LikeCommentRequestResponse()
					{
						message = "You have already liked that comment!",
						isError = false
					}.ToJson());
					stream.Close();
					return;
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
					HttpStream stream = new HttpStream(context.Response);
					stream.Send(new LikeCommentRequestResponse()
					{
						message = "You havent liked that comment.",
						isError = false
					}.ToJson());
					stream.Close();
					return;
				}
			}


			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(new LikeCommentRequestResponse()
			{
				message = "Your liked status has been updated!",
				isError = false
			}.ToJson());
			httpStream.Close();
		}

		[Serializable]
		private class LikeCommentRequestData
		{
			public bool likeState;
			public string modId;
			public string commentId;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(modId) && !string.IsNullOrWhiteSpace(commentId);
			}
		}
		[Serializable]
		private class LikeCommentRequestResponse
		{
			public string message;
			public bool isError;

			public string ToJson()
			{
				return JsonConvert.SerializeObject(this);
			}
		}
	}
}
