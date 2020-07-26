using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ModBotBackend.Users.Sessions
{
	public class Session
	{
		public Session(string ownerUserId)
		{
			byte[] generatedKey = new byte[16];
			new RNGCryptoServiceProvider().GetBytes(generatedKey);
			
			ExpieryTime = DateTime.Now.AddHours(1);
			Key = Convert.ToBase64String(generatedKey);
			OwnerUserID = ownerUserId;

			for(int i = SessionsManager.Sessions.Count - 1; i >= 0; i--)
			{
				if (SessionsManager.Sessions[i].OwnerUserID == ownerUserId)
				{
					SessionsManager.Sessions.RemoveAt(i);
				}
			}

			SessionsManager.Sessions.Add(this);
		}

		public bool IsExpierd()
		{
			return DateTime.Compare(ExpieryTime, DateTime.Now) <= -1;
		}
		public bool KeyMatches(string key)
		{
			return Key == key;
		}

		public readonly DateTime ExpieryTime;
		public readonly string Key;
		public readonly string OwnerUserID;
		
	}
}
