using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HttpUtils;
using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
	public class GetPublicUserDataOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context)
		{
			string id = context.Request.QueryString["id"];

			User user = UserManager.GetUserFromId(id);

			if (user == null)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(new PublicUserDataResponse()
				{
					message = "The user you asked for doesn't exist",
					isError = true
				}.ToJson());
				stream.Close();
				return;
			}

			PublicUserDataResponse publicUserData = new PublicUserDataResponse()
			{
				username = user.Username,
				bio = user.Bio,
				userID = user.UserID,
				favoritedMods = user.FavoritedMods,
				color = user.DisplayColor,

				isError = false,
				message = ""
			};

			context.Response.ContentType = "text/plain";
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(publicUserData.ToJson());
			httpStream.Close();
		}

		[Serializable]
		private class PublicUserDataResponse
		{
			public string username;
			public string bio;
			public string userID;
			public List<string> favoritedMods = new List<string>();
			public string color;

			public bool isError = false;
			public string message;

			public string ToJson()
			{
				return Newtonsoft.Json.JsonConvert.SerializeObject(this);
			}
		}
	}
}
