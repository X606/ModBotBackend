using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations
{
	[Operation("console")]
	public class OpenConsoleOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "text/html";

			if (!authentication.HasAtLeastAuthenticationLevel(Users.AuthenticationLevel.Admin))
			{
				HttpStream httpAccessDeniedStream = new HttpStream(context.Response);
				httpAccessDeniedStream.Send(Properties.Resources.ConsoleCantAccess);
				httpAccessDeniedStream.Close();
			}

			string html = Properties.Resources.Console;

			string css = ConsoleCustomCssManager.Instance.GetCssForUserID(authentication.UserID);
			if (css == null)
				css = Properties.Resources.ConsoleDefaultCss;

			html = Utils.FormatString(html, css);

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(html);
			httpStream.Close();
		}
	}
}
