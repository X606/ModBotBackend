using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HttpUtils;
using Newtonsoft.Json;
using ModLibrary;
using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
	[Operation("getAllModInfos")]
	public class GetAllModInfosOperation : JsonOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
		{
			string[] ids = UploadedModsManager.Instance.GetAllUploadedIds();

			ModInfo[] modInfos = new ModInfo[ids.Length];
			for(int i = 0; i < ids.Length; i++)
			{
				modInfos[i] = UploadedModsManager.Instance.GetModInfoFromId(ids[i]);
			}

			string json = JsonConvert.SerializeObject(modInfos);

			return new ModInfosResponse()
			{
				ModInfos = modInfos
			};
		}

	}
	public class ModInfosResponse : JsonOperationResponseBase
    {
		public ModInfo[] ModInfos;
    }
}
