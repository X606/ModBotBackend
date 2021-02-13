using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
    [Operation("getModData")]
    public class GetModDataOperation : PlainTextOperationBase
    {
        public override bool ParseAsJson => true;
        public override string[] Arguments => new string[] { "id" };
        public override bool ArgumentsInQuerystring => true;
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            string id = arguments["id"];

            return UploadedModsManager.Instance.GetModInfoJsonFromId(id);
        }

    }
}
