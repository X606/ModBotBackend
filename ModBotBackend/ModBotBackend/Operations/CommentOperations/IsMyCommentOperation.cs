using ModBotBackend.Users;
using System;

namespace ModBotBackend.Operations
{
    [Operation("isCommentMine")]
    public class IsMyCommentOperation : PlainTextOperationBase
    {
        public override bool ParseAsJson => true;
        public override string[] Arguments => new string[] { "modId", "commentId" };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            ContentType = "application/json";

            string modId = arguments["modId"];
            string commentId = arguments["commentId"];

            if (modId == null || commentId == null)
            {
                return "false";
            }

            if (!authentication.IsSignedIn)
            {
                return "false";
            }

            if (!UploadedModsManager.Instance.HasModWithIdBeenUploaded(modId))
            {
                return "false";
            }

            SpecialModData specialModData = UploadedModsManager.Instance.GetSpecialModInfoFromId(modId);

            Comment comment = specialModData.GetCommentWithCommentID(commentId);
            if (comment == null)
            {
                return "false";
            }

            string userId = authentication.UserID;

            bool isUs = comment.PosterUserId == userId;

            return isUs ? "true" : "false";
        }

        [Serializable]
        private class IsMyCommentRequestData
        {
            public string modId;
            public string commentId;

            public bool IsValidRequest()
            {
                return !string.IsNullOrWhiteSpace(modId) && !string.IsNullOrWhiteSpace(commentId);
            }
        }
    }
}
