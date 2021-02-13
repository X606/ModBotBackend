using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HttpUtils;
using Newtonsoft.Json;
using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
	[Operation("getAllModIds")]
	public class GetAllModIdsOperation : JsonOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
		public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
		{
			string[] ids = UploadedModsManager.Instance.GetAllUploadedIds();

			return new ModIdsResponse()
			{
				Ids = ids
			};
		}

	}

	public class ModIdsResponse : JsonOperationResponseBase
    {
		public string[] Ids;
    }

}
