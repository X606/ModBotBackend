using HttpUtils;
using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ModBotBackend.Operations.AdminOnly
{
	[Operation("setCustomConsoleCss")]
	public class SetCustomConsoleCssOperation : OperationBase
	{
		public override bool ParseAsJson => false;
		public override string[] Arguments => new string[] { "css" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.Admin;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			if (!authentication.HasAtLeastAuthenticationLevel(Users.AuthenticationLevel.Admin))
			{
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				context.Response.ContentType = "text/plain";
				HttpStream httapStream = new HttpStream(context.Response);
				httapStream.Send("Access denied :/");
				httapStream.Close();
				return;
			}

			string json = Encoding.UTF8.GetString(Misc.ToByteArray(context.Request.InputStream));

			Request data = JsonConvert.DeserializeObject<Request>(json);

			if (data.css == "[clear]")
			{
				ConsoleCustomCssManager.Instance.ClearCustomCssForUser(authentication.UserID);
			} else
			{
				ConsoleCustomCssManager.Instance.SetCustomCss(authentication.UserID, data.css);
			}

			

			context.Response.ContentType = "text/plain";
			HttpStream respone = new HttpStream(context.Response);
			respone.Send("Updated custom css for " + authentication.UserID);
			respone.Close();
		}

		[Serializable]
		public class Request
		{
			public string css;
		}


	}
}
