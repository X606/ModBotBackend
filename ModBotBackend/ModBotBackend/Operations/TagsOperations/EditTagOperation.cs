using ModBotBackend.Managers;
using ModBotBackend.Users;
using System.Net;

namespace ModBotBackend.Operations.TagsOperations
{
    [Operation("editTag")]
    public class EditTagOperation : JsonOperationBase
    {
        public override string[] Arguments => new string[] { "tagID", "body" };

        public override bool ParseAsJson => true;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;

        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.VerifiedUser))
            {
                StatusCode = HttpStatusCode.Unauthorized;
                return new Response()
                {
                    message = "You need to be at least a verified user to do this"
                };
            }

            Request request = new Request()
            {
                tagID = arguments["tagID"],
                body = arguments["body"]
            };

            TagInfo tag = TagsManager.Instance.GetTag(request.tagID);

            if (tag == null)
            {
                StatusCode = HttpStatusCode.BadRequest;
                return new Response()
                {
                    Error = "The requested tag doesn't exit"
                };
            }

            if (authentication.UserID != tag.CreatorId)
            {
                StatusCode = HttpStatusCode.Unauthorized;
                return new Response()
                {
                    Error = "You are not the owner of this tag."
                };
            }

            tag.Body = request.body;
            if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.Modder))
            {
                tag.Verified = false;
            }

            TagsManager.Instance.SaveTag(tag);

            return new Response()
            {
                message = "Updated tag."
            };
        }

        class Request
        {
            public string tagID;
            public string body;
        }
        class Response : JsonOperationResponseBase
        {
            public string message;
        }

    }
}
