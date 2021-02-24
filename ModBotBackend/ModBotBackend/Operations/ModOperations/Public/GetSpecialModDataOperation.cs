using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
    [Operation("getSpecialModData")]
    public class GetSpecialModDataOperation : PlainTextOperationBase
    {
        public override bool ParseAsJson => true;
        public override string[] Arguments => new string[] { "id" };
        public override bool ArgumentsInQuerystring => true;
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            ContentType = "application/json";

            string id = arguments["id"];

            return UploadedModsManager.Instance.GetSpecialModInfoJsonFromId(id);
        }

    }
}
