using ModBotBackend.Users;
using System;
using System.Collections.Generic;

namespace ModBotBackend.Operations
{
    [Operation("getUser")]
    public class GetPublicUserDataOperation : JsonOperationBase
    {
        public override bool ParseAsJson => true;
        public override string[] Arguments => new string[] { "id" };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
        public override bool ArgumentsInQuerystring => true;

        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            ContentType = "application/json";

            string id = arguments["id"];

            User user = UserManager.Instance.GetUserFromId(id);

            if (user == null)
            {
                return new PublicUserDataResponse()
                {
                    Error = "The user you asked for doesn't exist"
                };
            }

            string bio = user.Bio.Replace("\n", "<br>");

            PublicUserDataResponse publicUserData = new PublicUserDataResponse()
            {
                username = user.Username,
                bio = bio,
                userID = user.UserID,
                favoritedMods = user.FavoritedMods,
                color = user.DisplayColor,
                borderStyle = user.BorderStyle,
                showFull = user.ShowFull,
                authenticationLevel = (int)user.AuthenticationLevel
            };

            return publicUserData;
        }

        [Serializable]
        private class PublicUserDataResponse : JsonOperationResponseBase
        {
            public string username;
            public string bio;
            public string userID;
            public List<string> favoritedMods = new List<string>();
            public string color;
            public BorderStyles borderStyle;
            public bool showFull;
            public int authenticationLevel;
        }
    }
}
