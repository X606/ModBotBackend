using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Users.Sessions
{
	public static class SessionsManager
	{
		public static List<Session> Sessions = new List<Session>();

		public static void RemoveAllExpierdSessions()
		{
			for(int i = Sessions.Count - 1; i >= 0; i--)
			{
				if (Sessions[i].IsExpierd())
				{
					Sessions.RemoveAt(i);
				}
			}

		}

		public static void RemoveSession(string key)
		{
			for(int i = Sessions.Count - 1; i >= 0; i--)
			{
				if(Sessions[i].Key == key)
				{
					Sessions.RemoveAt(i);
					return;
				}
			}
		}

		public static bool VerifyKey(string key, out Session session)
		{
			foreach(Session selectedSession in Sessions)
			{
				if(selectedSession.Key == key && !selectedSession.IsExpierd())
				{
					session = selectedSession;
					return true;
				}
			}

			session = null;
			return false;
		}

		public static string GetPlayerIdFromSession(string sessionKey)
		{
			foreach(Session session in Sessions)
			{
				if (session.Key == sessionKey)
					return session.OwnerUserID;
				
			}

			return null;
		}
	}
}
