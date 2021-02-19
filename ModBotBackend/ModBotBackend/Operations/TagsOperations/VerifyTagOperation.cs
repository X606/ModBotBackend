using ModBotBackend.Managers;
using ModBotBackend.Users;
using System.Net;

namespace ModBotBackend.Operations.TagsOperations
{
    [Operation("verifyTag")]
    public class VerifyTagOperation : JsonOperationBase
    {
        public override string[] Arguments => new string[] { "tagID" };
        public override bool ArgumentsInQuerystring => true;
        public override bool ParseAsJson => true;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.Admin;

        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.Admin))
            {
                StatusCode = HttpStatusCode.Unauthorized;
                return new Response()
                {
                    Error = "You need to be an admin to do this"
                };
            }

            string tagID = arguments["tagID"];
            if (tagID == null)
            {
                StatusCode = HttpStatusCode.BadRequest;
                return new Response()
                {
                    Error = "You need to provide a tagID"
                };
            }

            TagInfo tag = TagsManager.Instance.GetTag(tagID);
            if (tag == null)
            {
                return new Response()
                {
                    Error = "The provided tagID is not accociated with a tag"
                };
            }

            if (tag.Verified)
            {
                return new Response()
                {
                    Error = "The provided tag is already verified."
                };
            }

            tag.Verified = true;
            TagsManager.Instance.SaveTag(tag);

            return new Response()
            {
                message = "Verified tag."
            };
        }

        class Response : JsonOperationResponseBase
        {
            public string message;
        }
    }
}
