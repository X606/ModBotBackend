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
	public class PostCommentOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "application/json";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			PostCommentRequest request = JsonConvert.DeserializeObject<PostCommentRequest>(json);

			if(!request.IsValidRequest())
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new PostCommentResponse()
				{
					message = "All fields were not filled out",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			if (!authentication.IsSignedIn)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new PostCommentResponse()
				{
					message = "You are not signed in.",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			if (!UploadedModsManager.Instance.HasModWithIdBeenUploaded(request.targetModId))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new PostCommentResponse()
				{
					message = "That mod does not exist",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			SpecialModData specialModData = UploadedModsManager.Instance.GetSpecialModInfoFromId(request.targetModId);

			string userId = authentication.UserID;

			string sanitized = request.commentBody.Replace("<", "&lt;").Replace(">", "&gt;");

			sanitized = sanitized.Replace("\n", "<br>");

			Comment comment = Comment.CreateNewComment(userId, sanitized);
			specialModData.Comments.Add(comment);
			specialModData.Save();

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(new PostCommentResponse()
			{
				message = "Comment posted.",
				isError = false
			}.ToJson());
			httpStream.Close();
		}

		[Serializable]
		private class PostCommentRequest
		{
			public string targetModId;
			public string commentBody;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(commentBody) && !string.IsNullOrWhiteSpace(targetModId);
			}
		}
		[Serializable]
		private class PostCommentResponse
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
