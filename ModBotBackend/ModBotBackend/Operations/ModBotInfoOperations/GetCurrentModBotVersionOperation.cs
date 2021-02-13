using ModBotBackend.Managers;
using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
    [Operation("getCurrentModBotVersion")]
    public class GetCurrentModBotVersionOperation : PlainTextOperationBase
    {
        public override string[] Arguments => new string[] { };

        public override bool ParseAsJson => false;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            return ModBotInfoManager.Instance.GetModBotVersion();
        }
    }
}
