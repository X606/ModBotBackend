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
	public class PostCommentOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context)
		{
			context.Response.ContentType = "text/plain";

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

			if (!SessionsManager.VerifyKey(request.sessionId, out Session session))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new PostCommentResponse()
				{
					message = "The provided session id was either invalid or outdated",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			if (!UploadedModsManager.HasModWithIdBeenUploaded(request.targetModId))
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

			SpecialModData specialModData = UploadedModsManager.GetSpecialModInfoFromId(request.targetModId);

			string userId = session.OwnerUserID;

			string sanitized = request.commentBody.Replace("<", "&lt;").Replace(">", "&gt;");

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
			public string sessionId;
			public string commentBody;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(sessionId) && !string.IsNullOrWhiteSpace(commentBody) && !string.IsNullOrWhiteSpace(targetModId);
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
