using ModBotBackend.Managers;
using ModBotBackend.Users;

namespace ModBotBackend.Operations.TagsOperations
{
    [Operation("setPlayerTags")]
    public class SetPlayerTags : JsonOperationBase
    {
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
        public override string[] Arguments => new string[] { "tags" };
        public override bool ParseAsJson => true;

        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            Request request = new Request()
            {
                tags = arguments["tags"]
            };

            if (!authentication.IsSignedIn)
            {
                return new Response()
                {
                    Error = "Not signed in"
                };
            }
            if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.VerifiedUser))
            {
                return new Response()
                {
                    Error = "You need to be verified to set your player tags"
                };
            }
            for (int i = 0; i < request.tags.Length; i++)
            {
                TagInfo tag = TagsManager.Instance.GetTag(request.tags[i]);
                if (!tag.Verified)
                {
                    return new Response()
                    {
                        Error = "The tag \"" + tag.TagID + "\" is not verified, so you cant use it yet."
                    };
                }
            }
            int maxTags = Utils.GetMaxPlayerTags(authentication);
            if (request.tags.Length > maxTags)
            {
                return new Response()
                {
                    Error = "You can only have a max of " + maxTags + " tags"
                };
            }

            User user = UserManager.Instance.GetUserFromId(authentication.UserID);

            TagsManager.Instance.SaveUserTags(user.PlayfabID, new PlayerTagsInfo(request.tags));

            return new Response()
            {
                message = "Updated tags for player with playfab id \"" + user.PlayfabID + "\""
            };
        }

        class Request
        {
            public string[] tags;
        }

        class Response : JsonOperationResponseBase
        {
            public string message;
        }

    }
}
