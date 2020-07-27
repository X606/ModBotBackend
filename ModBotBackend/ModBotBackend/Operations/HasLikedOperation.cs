﻿using System;
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
	public class HasLikedOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			LikeRequestData request = JsonConvert.DeserializeObject<LikeRequestData>(json);

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

			string userId = session.OwnerUserID;

			User user = UserManager.GetUserFromId(userId);

			bool hasLiked = user.LikedMods.Contains(request.modId);

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(hasLiked ? "true" : "false");
			httpStream.Close();
		}

		[Serializable]
		private class LikeRequestData
		{
			public string sessionId;
			public string modId;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(sessionId) && !string.IsNullOrWhiteSpace(modId);
			}
		}
	}
}