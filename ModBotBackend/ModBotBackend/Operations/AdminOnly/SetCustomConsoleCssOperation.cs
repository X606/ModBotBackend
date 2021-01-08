using HttpUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.AdminOnly
{
	[Operation("setCustomConsoleCss")]
	public class SetCustomConsoleCssOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			if (!authentication.HasAtLeastAuthenticationLevel(Users.AuthenticationLevel.Admin))
			{
				context.Response.ContentType = "text/plain";
				HttpStream httapStream = new HttpStream(context.Response);
				httapStream.Send("Access denied :/");
				httapStream.Close();
				return;
			}

			string data = Encoding.UTF8.GetString(Misc.ToByteArray(context.Request.InputStream));

			if (data == "[clear]")
			{
				ConsoleCustomCssManager.Instance.ClearCustomCssForUser(authentication.UserID);
			} else
			{
				ConsoleCustomCssManager.Instance.SetCustomCss(authentication.UserID, data);
			}

			

			context.Response.ContentType = "text/plain";
			HttpStream respone = new HttpStream(context.Response);
			respone.Send("Updated custom css for " + authentication.UserID);
			respone.Close();
		}
	}
}
