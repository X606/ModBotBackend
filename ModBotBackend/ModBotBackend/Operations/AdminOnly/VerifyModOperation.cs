using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using ModLibrary;

namespace ModBotBackend.Operations.AdminOnly
{
    [Operation("verifyMod")]
    public class VerifyModOperation : JsonOperationBase
    {
        public override string[] Arguments => new string[] { "modId" };

        public override bool ParseAsJson => true;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.Admin;

        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            SpecialModData modData = UploadedModsManager.Instance.GetSpecialModInfoFromId(arguments["modId"]);

            if (modData == null)
            {
                return new VerifyModResponse()
                {
                    Error = "Could not find mod"
                };
            }

            if (modData.Verified)
            {
                return new VerifyModResponse()
                {
                    Error = "Mod aready verified."
                };
            }

            modData.Verified = true;
            modData.Save();

            return new VerifyModResponse()
            {
                Message = "Verified mod."
            };
        }


        class VerifyModResponse : JsonOperationResponseBase
        {
            public string Message;
        }

    }
}
