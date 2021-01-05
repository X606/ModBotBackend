using ModBotBackend.Users;

namespace ModBotBackend
{
	public class Authentication
	{
		public Authentication(AuthenticationLevel authenticationLevel, string userID, string sessionID)
		{
			AuthenticationLevel = authenticationLevel;
			UserID = userID;
			SessionID = sessionID;
		}

		public bool HasAtLeastAuthenticationLevel(AuthenticationLevel minLevel) => AuthenticationLevel >= minLevel;

		public bool IsSignedIn => HasAtLeastAuthenticationLevel(AuthenticationLevel.BasicUser);

		public readonly AuthenticationLevel AuthenticationLevel;
		public readonly string UserID;
		public readonly string SessionID;
	}
}
