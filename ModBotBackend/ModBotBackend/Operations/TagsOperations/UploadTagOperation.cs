using ModBotBackend.Managers;
using ModBotBackend.Users;

namespace ModBotBackend.Operations.TagsOperations
{
    [Operation("uploadTag")]
    public class UploadTagOperation : JsonOperationBase
    {
        public override string[] Arguments => new string[] { "tagName", "tagBody" };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
        public override bool ParseAsJson => true;

        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            if (!authentication.IsSignedIn)
            {
                return new Response()
                {
                    Error = "You are not logged in"
                };
            }
            if (authentication.AuthenticationLevel < AuthenticationLevel.VerifiedUser)
            {
                return new Response()
                {
                    Error = "To prevent spam you need to have a verified account to upload tags"
                };
            }
            Request request = new Request()
            {
                tagBody = arguments["tagBody"],
                tagName = arguments["tagName"]
            };

            TagInfo tag = new TagInfo(authentication.UserID, request.tagName, request.tagBody);
            if (authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.Modder))
            {
                tag.Verified = true;
            }

            TagsManager.Instance.SaveTag(tag);

            return new Response()
            {
                message = "Uploaded tag!",
                ID = tag.TagID
            };
        }

        public class Request
        {
            public string tagBody;
            public string tagName;
        }
        public class Response : JsonOperationResponseBase
        {
            public string message = null;
            public string ID = null;
        }

    }
}
