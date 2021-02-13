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
	[Operation("getModBotLauncherDownload")]
	public class GetModBotLauncherDownloadLinkOperation : PlainTextOperationBase
	{
		public override string[] Arguments => new string[] { };

		public override bool ParseAsJson => false;

		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override string OnOperation(Arguments arguments, Authentication authentication)
		{
			return ModBotInfoManager.Instance.GetModBotLauncherDownloadLink();
		}
	}
}
