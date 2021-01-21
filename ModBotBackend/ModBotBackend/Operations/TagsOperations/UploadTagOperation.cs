using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend.Managers;

namespace ModBotBackend.Operations.TagsOperations
{
	[Operation("uploadTag")]
	public class UploadTagOperation : OperationBase
	{
		public override string[] Arguments => new string[] { "tagName", "tagBody" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
		public override bool ParseAsJson => true;
		
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			if (!authentication.IsSignedIn)
			{
				Utils.Respond(context.Response, new Response()
				{
					error = "You are not logged in"
				});
				return;
			}
			if (authentication.AuthenticationLevel < AuthenticationLevel.VerifiedUser)
			{
				Utils.Respond(context.Response, new Response()
				{
					error = "To prevent spam you need to have a verified account to upload tags"
				});
				return;
			}
			if (!Utils.TryGetRequestBody<Request>(context, out Request request))
			{
				Utils.Respond(context.Response, new Response()
				{
					error = "the request was invalid"
				});
				return;
			}

			TagInfo tag = new TagInfo(authentication.UserID, request.tagName, request.tagBody);
			if (authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.Modder))
			{
				tag.Verified = true;
			}

			TagsManager.Instance.SaveTag(tag);

			Utils.Respond(context.Response, new Response()
			{
				message = "Uploaded tag!",
				ID = tag.TagID
			});
		}

		public class Request
		{
			public string tagBody;
			public string tagName;
		}
		public class Response
		{
			public string error = null;
			public string message = null;
			public string ID = null;
		}

	}
}
