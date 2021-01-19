using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ModBotBackend.Managers;

namespace ModBotBackend.Operations.TagsOperations
{
	[Operation("getPlayerPrefix")]
	public class GetPrefixForPlayfabIdOperation : OperationBase
	{
		public override string[] Arguments => new string[] { "playfabID" };
		public override bool ArgumentsInQuerystring => true;
		public override bool ParseAsJson => true;

		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string playfabID = context.Request.QueryString["playfabID"];

			TagInfo[] tags = TagsManager.Instance.GetTagsForPlayfabId(playfabID);

			StringBuilder prefixBuilder = new StringBuilder();
			for (int i = 0; i < tags.Length; i++)
			{
				if (tags[i].Verified)
				{
					prefixBuilder.Append(tags[i].Body);

					prefixBuilder.Append(" ");
				}
			}

			string json = JsonConvert.SerializeObject(new Result()
			{
				nameOverride = null,
				prefix = prefixBuilder.ToString().Trim(" ".ToCharArray())
			});

			TagsManager.Instance.GetTagsForPlayfabId(playfabID);

			context.Response.ContentType = "application/json";
			HttpStream stream = new HttpStream(context.Response);
			stream.Send(json);
			stream.Close();
		}

		class Result
		{
			public string nameOverride;
			public string prefix;
		}

	}
}
