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
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "application/json";

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

			string bio = user.Bio.Replace("\n", "<br>");

			PublicUserDataResponse publicUserData = new PublicUserDataResponse()
			{
				username = user.Username,
				bio = bio,
				userID = user.UserID,
				favoritedMods = user.FavoritedMods,
				color = user.DisplayColor,
				borderStyle = user.BorderStyle,
                showFull = user.ShowFull,

                isError = false,
				message = ""
			};

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
			public BorderStyles borderStyle;
            public bool showFull;

			public bool isError = false;
			public string message;

			public string ToJson()
			{
				return Newtonsoft.Json.JsonConvert.SerializeObject(this);
			}
		}
	}
}
