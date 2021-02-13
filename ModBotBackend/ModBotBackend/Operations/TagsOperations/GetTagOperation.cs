using ModBotBackend.Managers;
using ModBotBackend.Users;

namespace ModBotBackend.Operations.TagsOperations
{
    [Operation("getTag")]
    public class GetTagOperation : JsonOperationBase
    {
        public override string[] Arguments => new string[] { "tagID" };
        public override bool ArgumentsInQuerystring => true;
        public override bool ParseAsJson => true;
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            string tagID = arguments["tagID"];

            if (tagID == null)
            {
                return null;
            }
            TagInfo tag = TagsManager.Instance.GetTag(tagID);

            return new GetTagResponse()
            {
                Tag = tag
            };
        }


    }
    public class GetTagResponse : JsonOperationResponseBase
    {
        public TagInfo Tag;
    }
}
