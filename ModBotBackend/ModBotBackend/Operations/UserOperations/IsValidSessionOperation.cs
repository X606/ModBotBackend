using ModBotBackend.Users;
using ModBotBackend.Users.Sessions;

namespace ModBotBackend.Operations
{
    [Operation("isValidSession")]
    public class IsValidSessionOperation : PlainTextOperationBase
    {
        public override bool ParseAsJson => false;
        public override string[] Arguments => new string[] { "sessionId" };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            string sessionId = arguments["sessionId"];

            if (sessionId == null)
                return "false";

            if (!SessionsManager.Instance.VerifyKey(sessionId, out Session session))
                return "false";

            return "true";
        }

    }
}
