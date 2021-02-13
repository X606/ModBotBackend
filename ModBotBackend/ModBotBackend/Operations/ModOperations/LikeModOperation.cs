using ModBotBackend.Users;
using System;

namespace ModBotBackend.Operations
{
    [Operation("like")]
    public class LikeModOperation : JsonOperationBase
    {
        public override bool ParseAsJson => true;
        public override string[] Arguments => new string[] { "likedModId", "likeState" };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            LikeRequestData request = new LikeRequestData()
            {
                likedModId = arguments["likedModId"],
                likeState = arguments["likeState"]
            };

            if (!request.IsValidRequest())
            {
                return new LikeRequestResponse()
                {
                    Error = "All fields were not filled out"
                };
            }

            if (!authentication.IsSignedIn)
            {
                return new LikeRequestResponse()
                {
                    Error = "You are not signed in."
                };
            }

            if (!UploadedModsManager.Instance.HasModWithIdBeenUploaded(request.likedModId))
            {
                return new LikeRequestResponse()
                {
                    Error = "No mod with that id exists"
                };
            }

            string userId = authentication.UserID;

            User user = UserManager.Instance.GetUserFromId(userId);
            SpecialModData modData = UploadedModsManager.Instance.GetSpecialModInfoFromId(request.likedModId);

            if (request.likeState)
            {
                if (!user.LikedMods.Contains(request.likedModId))
                {
                    user.LikedMods.Add(request.likedModId);
                    user.Save();
                    modData.Likes++;
                    modData.Save();
                }
                else
                {
                    return new LikeRequestResponse()
                    {
                        Error = "You have already liked that mod!"
                    };
                }

            }
            else
            {
                if (user.LikedMods.Contains(request.likedModId))
                {
                    user.LikedMods.Remove(request.likedModId);
                    user.Save();
                    modData.Likes--;
                    modData.Save();
                }
                else
                {
                    return new LikeRequestResponse()
                    {
                        Error = "You havent liked that mod!"
                    };
                }
            }

            return new LikeRequestResponse()
            {
                message = "Your liked status has been updated!"
            };
        }

        [Serializable]
        private class LikeRequestData
        {
            public bool likeState;
            public string likedModId;

            public bool IsValidRequest()
            {
                return !string.IsNullOrWhiteSpace(likedModId);
            }
        }
        [Serializable]
        private class LikeRequestResponse : JsonOperationResponseBase
        {
            public string message;
        }
    }
}
