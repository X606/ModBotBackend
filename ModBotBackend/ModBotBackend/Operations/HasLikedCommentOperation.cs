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
	public class HasLikedCommentOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			LikeCommentRequestData request = JsonConvert.DeserializeObject<LikeCommentRequestData>(json);

			if(!request.IsValidRequest())
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send("false");
				stream.Close();
				return;
			}

			if(!SessionsManager.VerifyKey(request.sessionId, out Session session))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send("false");
				stream.Close();
				return;
			}

			if(!UploadedModsManager.HasModWithIdBeenUploaded(request.modId))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send("false");
				stream.Close();
				return;
			}

			SpecialModData specialModData = UploadedModsManager.GetSpecialModInfoFromId(request.modId);

			Comment comment = specialModData.GetCommentWithCommentID(request.commentId);
			if (comment == null)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send("false");
				stream.Close();
				return;
			}
			
			string userId = session.OwnerUserID;

			bool hasLiked = comment.UsersWhoLikedThis.Contains(userId);

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(hasLiked ? "true" : "false");
			httpStream.Close();
		}

		[Serializable]
		private class LikeCommentRequestData
		{
			public string sessionId;
			public string modId;
			public string commentId;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(sessionId) && !string.IsNullOrWhiteSpace(modId) && !string.IsNullOrWhiteSpace(commentId);
			}
		}
	}
}
