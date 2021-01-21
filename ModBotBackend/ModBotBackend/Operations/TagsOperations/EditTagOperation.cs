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
	[Operation("editTag")]
	public class EditTagOperation : OperationBase
	{
		public override string[] Arguments => new string[] { "tagID", "body" };

		public override bool ParseAsJson => true;

		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.VerifiedUser))
			{
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "You need to be at least a verified user to do this"
				});
				return;
			}

			if (!Utils.TryGetRequestBody(context, out Request request))
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "Invalid request"
				});
				return;
			}

			TagInfo tag = TagsManager.Instance.GetTag(request.tagID);

			if (tag == null)
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "The requested tag doesn't exit"
				});
				return;
			}

			if (authentication.UserID != tag.CreatorId)
			{
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				Utils.Respond(context.Response, new Response()
				{
					isError = true,
					message = "You are not the owner of this tag."
				});
				return;
			}

			tag.Body = request.body;
			if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.Modder))
			{
				tag.Verified = false;
			}

			TagsManager.Instance.SaveTag(tag);

			Utils.Respond(context.Response, new Response()
			{
				isError = false,
				message = "Updated tag."
			});
		}

		class Request
		{
			public string tagID;
			public string body;
		}
		class Response
		{
			public bool isError;
			public string message;
		}

	}
}
