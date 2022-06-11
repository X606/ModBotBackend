using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.ModOperations.Public
{
    [Operation("isModVerified")]
    public class IsModVerifiedOperation : PlainTextOperationBase
    {
        public override string[] Arguments => new string[] { "modId" };

        public override bool ParseAsJson => true;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override bool ArgumentsInQuerystring => true;

        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            string modId = arguments["modId"];

            if (modId == null)
                return "false";

            SpecialModData specialModData = UploadedModsManager.Instance.GetSpecialModInfoFromId(modId);

            if (specialModData == null)
                return "false";

            return specialModData.Verified ? "true" : "false";
        }
    }
}
