using ModBotBackend.Users;
using System;

namespace ModBotBackend.Operations
{
    [Operation("hasLiked")]
    public class HasLikedModOperation : PlainTextOperationBase
    {
        public override bool ParseAsJson => true;
        public override string[] Arguments => new string[] { "modId" };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            ContentType = "application/json";

            LikeRequestData request = new LikeRequestData()
            {
                modId = arguments["modId"]
            };

            if (!request.IsValidRequest())
            {
                return "false";
            }

            if (!authentication.IsSignedIn)
            {
                return "false";
            }

            if (!UploadedModsManager.Instance.HasModWithIdBeenUploaded(request.modId))
            {
                return "false";
            }

            string userId = authentication.UserID;

            User user = UserManager.Instance.GetUserFromId(userId);

            bool hasLiked = user.LikedMods.Contains(request.modId);

            return hasLiked ? "true" : "false";
        }

        [Serializable]
        private class LikeRequestData
        {
            public string modId;

            public bool IsValidRequest()
            {
                return !string.IsNullOrWhiteSpace(modId);
            }
        }
    }
}
