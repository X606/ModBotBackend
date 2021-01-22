using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend.Managers;

namespace ModBotBackend.Operations
{
	[Operation("getModBotDownload")]
	public class GetModBotDownloadLinkOperation : OperationBase
	{
		public override string[] Arguments => new string[] { };

		public override bool ParseAsJson => false;

		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "text/plain";

			string modbotDownload = ModBotInfoManager.Instance.GetModBotDownloadLink();

			Utils.Respond(context.Response, modbotDownload);

		}
	}
}
