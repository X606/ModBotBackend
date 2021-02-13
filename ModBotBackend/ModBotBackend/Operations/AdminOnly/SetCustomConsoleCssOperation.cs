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
	public class SetCustomConsoleCssOperation : PlainTextOperationBase
	{
		public override bool ParseAsJson => false;
		public override string[] Arguments => new string[] { "css" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.Admin;

		public override string OnOperation(Arguments arguments, Authentication authentication)
		{
			ContentType = "text/plain";

			if (!authentication.HasAtLeastAuthenticationLevel(Users.AuthenticationLevel.Admin))
			{
				StatusCode = HttpStatusCode.Unauthorized;

				return "Access denied :/";
			}

			string css = arguments["css"];

			if (css == "[clear]")
			{
				ConsoleCustomCssManager.Instance.ClearCustomCssForUser(authentication.UserID);
			} else
			{
				ConsoleCustomCssManager.Instance.SetCustomCss(authentication.UserID, css);
			}

			return "Updated custom css for " + authentication.UserID;
		}


	}
}
