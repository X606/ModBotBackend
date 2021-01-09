using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations
{
	[Operation("getMyAuth")]
	public class GetMyAuthenticationLevelOperation : OperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "text/plain";
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(((int)authentication.AuthenticationLevel).ToString());
			httpStream.Close();
		}
	}
}
