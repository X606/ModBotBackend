using ModBotBackend.Managers;
using ModBotBackend.Users;
using System.Net;

namespace ModBotBackend.Operations.TagsOperations
{
    [Operation("getMyTags")]
    public class GetMyTagsOperation : JsonOperationBase
    {
        public override string[] Arguments => new string[] { };

        public override bool ParseAsJson => true;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;

        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.VerifiedUser))
            {
                StatusCode = HttpStatusCode.Unauthorized;
                return new Result()
                {
                    Error = "Unauthorized"
                };
            }

            User user = UserManager.Instance.GetUserFromId(authentication.UserID);

            TagInfo[] tags = TagsManager.Instance.GetTagsForPlayfabId(user.PlayfabID);

            string[] tagIds = new string[tags.Length];
            for (int i = 0; i < tags.Length; i++)
            {
                tagIds[i] = tags[i].TagID;
            }

            return new Result()
            {
                Tags = tagIds
            };
        }


        class Result : JsonOperationResponseBase
        {
            public string[] Tags;
        }
    }
}
