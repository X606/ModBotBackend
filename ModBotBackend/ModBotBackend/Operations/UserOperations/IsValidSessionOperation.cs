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
	[Operation("isValidSession")]
	public class IsValidSessionOperation : PlainTextOperationBase
	{
		public override bool ParseAsJson => false;
		public override string[] Arguments => new string[] { "sessionId" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
		public override string OnOperation(Arguments arguments, Authentication authentication)
		{
			string sessionId = arguments["sessionId"];

			if (sessionId == null)
				return "false";

			if(!SessionsManager.Instance.VerifyKey(sessionId, out Session session))
				return "false";

			return "true";
		}

	}
}
