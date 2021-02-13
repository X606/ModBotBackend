using ModBotBackend.Managers;
using ModBotBackend.Users;

namespace ModBotBackend.Operations.TagsOperations
{
    [Operation("getTags")]
    public class GetTagsOperation : JsonOperationBase
    {
        public override string[] Arguments => new string[] { };

        public override bool ParseAsJson => true;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            TagInfo[] tags = TagsManager.Instance.GetTags();

            return new Result()
            {
                tags = tags
            };
        }

        class Result : JsonOperationResponseBase
        {
            public TagInfo[] tags;
        }

    }
}
