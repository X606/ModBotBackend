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
	[Operation("hasLiked")]
	public class HasLikedModOperation : OperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "modId" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
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

			if(!authentication.IsSignedIn)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send("false");
				stream.Close();
				return;
			}

			if(!UploadedModsManager.Instance.HasModWithIdBeenUploaded(request.modId))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send("false");
				stream.Close();
				return;
			}

			string userId = authentication.UserID;

			User user = UserManager.Instance.GetUserFromId(userId);

			bool hasLiked = user.LikedMods.Contains(request.modId);

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(hasLiked ? "true" : "false");
			httpStream.Close();
		}

		[Serializable]
		private class LikeRequestData
		{
			public string modId;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(modId);
			}
		}
	}
}
