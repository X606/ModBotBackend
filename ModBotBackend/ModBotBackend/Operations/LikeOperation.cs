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
	[Operation("like")]
	public class LikeOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			LikeRequestData request = JsonConvert.DeserializeObject<LikeRequestData>(json);

			if(!request.IsValidRequest())
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new LikeRequestResponse()
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
				stream.Send(new LikeRequestResponse()
				{
					message = "You are not signed in.",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			if (!UploadedModsManager.HasModWithIdBeenUploaded(request.likedModId))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new LikeRequestResponse()
				{
					message = "No mod with that id exists",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			string userId = authentication.UserID;

			User user = UserManager.GetUserFromId(userId);
			SpecialModData modData = UploadedModsManager.GetSpecialModInfoFromId(request.likedModId);

			if (request.likeState)
			{
				if (!user.LikedMods.Contains(request.likedModId))
				{
					user.LikedMods.Add(request.likedModId);
					user.Save();
					modData.Likes++;
					modData.Save();
				}
				else
				{
					HttpStream stream = new HttpStream(context.Response);
					stream.Send(new LikeRequestResponse()
					{
						message = "You have already liked that mod!",
						isError = false
					}.ToJson());
					stream.Close();
					return;
				}
				
			} else
			{
				if(user.LikedMods.Contains(request.likedModId))
				{
					user.LikedMods.Remove(request.likedModId);
					user.Save();
					modData.Likes--;
					modData.Save();
				}
				else
				{
					HttpStream stream = new HttpStream(context.Response);
					stream.Send(new LikeRequestResponse()
					{
						message = "You havent liked that mod!",
						isError = false
					}.ToJson());
					stream.Close();
					return;
				}
			}
			

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(new LikeRequestResponse()
			{
				message = "Your liked status has been updated!",
				isError = false
			}.ToJson());
			httpStream.Close();
		}

		[Serializable]
		private class LikeRequestData
		{
			public bool likeState;
			public string likedModId;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(likedModId);
			}
		}
		[Serializable]
		private class LikeRequestResponse
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
