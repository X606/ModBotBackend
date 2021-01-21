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
	[Operation("verifyTag")]
	public class VerifyTagOperation : OperationBase
	{
		public override string[] Arguments => new string[] { "tagID" };
		public override bool ArgumentsInQuerystring => true;
		public override bool ParseAsJson => true;

		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.Admin;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			if(!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.Admin))
			{
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "You need to be an admin to do this"
				});
				return;
			}

			string tagID = context.Request.QueryString["tagID"];
			if (tagID == null)
			{
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "You need to provide a tagID"
				});
				return;
			}

			TagInfo tag = TagsManager.Instance.GetTag(tagID);
			if (tag == null)
			{
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "The provided tagID is not accociated with a tag"
				});
				return;
			}

			if(tag.Verified)
			{
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "The provided tag is already verified."
				});
				return;
			}

			tag.Verified = true;
			TagsManager.Instance.SaveTag(tag);

			Utils.Respond(context.Response, new Response()
			{
				isError = false,
				message = "Verified tag."
			});

		}

		class Response
		{
			public bool isError;
			public string message;
		}
	}
}
