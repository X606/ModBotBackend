using ModBotBackend.Managers;
using ModBotBackend.Users;

namespace ModBotBackend
{
    public class Authentication
    {
        public Authentication(AuthenticationLevel authenticationLevel, string userID, string sessionID, string ip)
        {
            AuthenticationLevel = authenticationLevel;
            UserID = userID;
            SessionID = sessionID;
            IP = ip;
        }

        public bool HasAtLeastAuthenticationLevel(AuthenticationLevel minLevel) => AuthenticationLevel >= minLevel;

        public bool IsSignedIn => HasAtLeastAuthenticationLevel(AuthenticationLevel.BasicUser);

        public bool IsBanned => BannedUsersManager.Instance.IsUserBanned(UserID);

        public readonly AuthenticationLevel AuthenticationLevel;
        public readonly string UserID;
        public readonly string SessionID;
        public readonly string IP;
    }
}
