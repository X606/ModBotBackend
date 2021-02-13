using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
    [Operation("getAllModIds")]
    public class GetAllModIdsOperation : JsonOperationBase
    {
        public override bool ParseAsJson => true;
        public override string[] Arguments => new string[] { };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            string[] ids = UploadedModsManager.Instance.GetAllUploadedIds();

            return new ModIdsResponse()
            {
                Ids = ids
            };
        }

    }

    public class ModIdsResponse : JsonOperationResponseBase
    {
        public string[] Ids;
    }

}
