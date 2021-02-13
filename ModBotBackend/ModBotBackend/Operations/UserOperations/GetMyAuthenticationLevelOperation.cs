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
	public class GetMyAuthenticationLevelOperation : PlainTextOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
		public override string OnOperation(Arguments arguments, Authentication authentication)
		{
			return ((int)authentication.AuthenticationLevel).ToString();
		}
	}
}
