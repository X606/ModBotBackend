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
	public class GetTagsOperation : JsonOperationBase
	{
		public override string[] Arguments => new string[] { };

		public override bool ParseAsJson => true;

		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
		{
			TagInfo[] tags = TagsManager.Instance.GetTags();

			return new Result()
			{
				tags = tags
			};
		}

		class Result : JsonOperationResponseBase
		{
			public TagInfo[] tags;
		}

	}
}
