using ModBotBackend.Managers;
using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.UserOperations.Public
{
    [Operation("isUserBanned")]
    public class IsUserBannedOperation : PlainTextOperationBase
    {
        public override string[] Arguments => new string[] { "userId" };

        public override bool ParseAsJson => false;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            ContentType = "application/json";

            return BannedUsersManager.Instance.IsUserBanned(arguments["userId"]) ? "true" : "false";
        }
    }
}
