using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
    [Operation("getCurrentUser")]
    public class GetCurrentUserOperation : PlainTextOperationBase
    {
        public override bool ParseAsJson => false;
        public override string[] Arguments => new string[] { };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;

        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            ContentType = "text/plain";

            if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.BasicUser))
            {
                return "null";
            }

            return authentication.UserID;
        }

    }
}
