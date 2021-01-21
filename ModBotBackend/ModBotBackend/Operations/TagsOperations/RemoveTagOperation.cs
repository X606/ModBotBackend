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
	[Operation("removeTag")]
	public class RemoveTagOperation : OperationBase
	{
		public override string[] Arguments => new string[] { "tagID" };

		public override bool ParseAsJson => true;

		public override bool ArgumentsInQuerystring => true;

		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.VerifiedUser;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string tagId = context.Request.QueryString["tagID"];

			if (tagId == null)
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "Invalid request"
				});
				return;
			}

			TagInfo tag = TagsManager.Instance.GetTag(tagId);
			if (tag == null)
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "Could not find the requested tag"
				});
				return;
			}

			bool authorized = false;

			if (authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.Admin))
				authorized = true;

			if (tag.CreatorId == authentication.UserID)
				authorized = true;

			if (!authorized)
			{
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "Unauthorized"
				});
				return;
			}

			TagsManager.Instance.RemoveTag(tag);

			context.Response.StatusCode = (int)HttpStatusCode.OK;
			Utils.Respond(context.Response, new Response()
			{
				isError = false,
				message = "Removed tag."
			});
		}
		
		class Response
		{
			public bool isError;
			public string message;
		}

	}

}
