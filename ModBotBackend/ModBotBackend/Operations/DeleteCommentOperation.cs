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
	public class DeleteCommentOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			DeleteCommentRequest request = JsonConvert.DeserializeObject<DeleteCommentRequest>(json);

			if(!request.IsValidRequest())
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new DeleteCommentResponse()
				{
					message = "All fields were not filled out",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			if(!SessionsManager.VerifyKey(request.sessionId, out Session session))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new DeleteCommentResponse()
				{
					message = "The provided session id was either invalid or outdated",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			if(!UploadedModsManager.HasModWithIdBeenUploaded(request.targetModId))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new DeleteCommentResponse()
				{
					message = "That mod does not exist",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			SpecialModData specialModData = UploadedModsManager.GetSpecialModInfoFromId(request.targetModId);

			string userId = session.OwnerUserID;

			Comment comment = specialModData.GetCommentWithCommentID(request.targetCommentId);
			if (comment == null)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new DeleteCommentResponse()
				{
					message = "There is no comment with that id on that mod",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			if (session.OwnerUserID != comment.PosterUserId)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new DeleteCommentResponse()
				{
					message = "You do not have premission to delete this comment",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			specialModData.DeleteCommentWithId(request.targetCommentId);
			specialModData.Save();

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(new DeleteCommentResponse()
			{
				message = "Comment deleted.",
				isError = false
			}.ToJson());
			httpStream.Close();
		}

		[Serializable]
		private class DeleteCommentRequest
		{
			public string targetModId;
			public string sessionId;
			public string targetCommentId;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(sessionId) && !string.IsNullOrWhiteSpace(targetModId);
			}
		}
		[Serializable]
		private class DeleteCommentResponse
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
