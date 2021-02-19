using ModBotBackend.Users;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.AdminOnly
{
    [Operation("removeMod")]
    public class RemoveModOperation : JsonOperationBase
    {
        public override string[] Arguments => new string[] { "modId" };

        public override bool ParseAsJson => true;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.Admin;

        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            string modId = arguments["modId"];

            ModInfo modInfo = UploadedModsManager.Instance.GetModInfoFromId(modId);

            if (modInfo == null)
            {
                return new RemoveModResponse()
                {
                    Error = "Mod not found"
                };
            }

            UploadedModsManager.Instance.DeleteMod(modId);

            return new RemoveModResponse()
            {
                Message = "Removed mod."
            };
        }


        class RemoveModResponse : JsonOperationResponseBase
        {
            public string Message;
        }
    }
}
