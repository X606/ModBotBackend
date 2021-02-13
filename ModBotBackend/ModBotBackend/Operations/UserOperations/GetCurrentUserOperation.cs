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
	[Operation("getCurrentUser")]
	public class GetCurrentUserOperation : PlainTextOperationBase
	{
		public override bool ParseAsJson => false;
		public override string[] Arguments => new string[] { };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;

		public override string OnOperation(Arguments arguments, Authentication authentication)
		{
			ContentType = "text/plain";

			if(!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.BasicUser))
			{
				return "null";
			}

			return authentication.UserID;
		}

	}
}
