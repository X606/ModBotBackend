using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend.Managers;
using ModBotBackend.Users;

namespace ModBotBackend.Operations.TagsOperations
{
	[Operation("getTag")]
	public class GetTagOperation : OperationBase
	{
		public override string[] Arguments => new string[] { "tagID" };
		public override bool ArgumentsInQuerystring => true;
		public override bool ParseAsJson => true;
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string tagID =  context.Request.QueryString["tagID"];
			
			if (tagID == null)
			{
				Utils.Respond(context.Response, "null");
				return;
			}
			TagInfo tag = TagsManager.Instance.GetTag(tagID);
			
			if (tag == null)
			{
				Utils.Respond(context.Response, "null");
				return;
			}
			
			Utils.Respond(context.Response, tag);
		}


	}
}
