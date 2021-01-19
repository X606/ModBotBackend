using ModBotBackend.Managers;
using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.TagsOperations
{
	[Operation("getTags")]
	public class GetTagsOperation : OperationBase
	{
		public override string[] Arguments => new string[] { };

		public override bool ParseAsJson => true;

		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			TagInfo[] tags = TagsManager.Instance.GetTags();

			Utils.Respond(context.Response, new Result()
			{
				tags = tags
			});
		}

		class Result
		{
			public TagInfo[] tags;
		}

	}
}
