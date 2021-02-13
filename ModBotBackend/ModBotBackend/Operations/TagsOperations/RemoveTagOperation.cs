using ModBotBackend.Managers;
using ModBotBackend.Users;
using System.Net;

namespace ModBotBackend.Operations.TagsOperations
{
    [Operation("removeTag")]
    public class RemoveTagOperation : JsonOperationBase
    {
        public override string[] Arguments => new string[] { "tagID" };

        public override bool ParseAsJson => true;

        public override bool ArgumentsInQuerystring => true;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.VerifiedUser;

        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            string tagId = arguments["tagID"];

            if (tagId == null)
            {
                StatusCode = HttpStatusCode.BadRequest;
                return new Response()
                {
                    Error = "Invalid request"
                };
            }

            TagInfo tag = TagsManager.Instance.GetTag(tagId);
            if (tag == null)
            {
                StatusCode = HttpStatusCode.NotFound;
                return new Response()
                {
                    Error = "Could not find the requested tag"
                };
            }

            bool authorized = false;

            if (authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.Admin))
                authorized = true;

            if (tag.CreatorId == authentication.UserID)
                authorized = true;

            if (!authorized)
            {
                StatusCode = HttpStatusCode.Unauthorized;
                return new Response()
                {
                    Error = "Unauthorized"
                };
            }

            TagsManager.Instance.RemoveTag(tag);

            StatusCode = HttpStatusCode.OK;
            return new Response()
            {
                message = "Removed tag."
            };
        }

        class Response : JsonOperationResponseBase
        {
            public string message;
        }

    }

}
