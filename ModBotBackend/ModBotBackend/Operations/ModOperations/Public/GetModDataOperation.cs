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
	[Operation("getModData")]
	public class GetModDataOperation : PlainTextOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "id"};
		public override bool ArgumentsInQuerystring => true;
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override string OnOperation(Arguments arguments, Authentication authentication)
		{
		 	string id = arguments["id"];

			return UploadedModsManager.Instance.GetModInfoJsonFromId(id);
		}

	}
}
