using System;
using System.Security.Cryptography;
using System.Text;

namespace ModBotBackend.Users.Sessions
{
    public class Session
    {
        public Session(string ownerUserId, float validDays)
        {
            byte[] generatedKey = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(generatedKey);

            ExpieryTime = DateTime.Now.AddDays(validDays);
            Key = Convert.ToBase64String(generatedKey);
            OwnerUserID = ownerUserId;
            AuthenticationLevel = UserManager.Instance.GetUserFromId(ownerUserId).AuthenticationLevel;

            AddToSessionsManager(ownerUserId);
        }

        public Session(byte[] data)
        {
            int index = 0;

            ExpieryTime = new DateTime(BitConverter.ToInt64(data, index));
            index += sizeof(long);

            byte[] key = new byte[16];
            Array.Copy(data, index, key, 0, 16);
            index += 16;
            Key = Convert.ToBase64String(key);

            int ownerIDLength = BitConverter.ToInt32(data, index);
            index += sizeof(int);

            byte[] ownerUserID = new byte[ownerIDLength];
            Array.Copy(data, index, ownerUserID, 0, ownerIDLength);
            index += ownerIDLength;
            OwnerUserID = Encoding.UTF8.GetString(ownerUserID);

            AuthenticationLevel = UserManager.Instance.GetUserFromId(OwnerUserID).AuthenticationLevel;

            AddToSessionsManager(OwnerUserID);
        }
        public void AddToSessionsManager(string ownerUserId)
        {
            SessionsManager.Instance.Sessions.Add(this);
        }

        public byte[] ToData()
        {
            int index = 0;

            byte[] ownerUserIDBytes = Encoding.UTF8.GetBytes(OwnerUserID);

            int size = sizeof(long) + 16 + sizeof(int) + ownerUserIDBytes.Length;
            byte[] data = new byte[size];

            Array.Copy(BitConverter.GetBytes(ExpieryTime.Ticks), 0, data, index, sizeof(long));
            index += sizeof(long);

            byte[] keyAsBytes = Convert.FromBase64String(Key);
            Array.Copy(keyAsBytes, 0, data, index, 16);
            index += 16;

            Array.Copy(BitConverter.GetBytes(ownerUserIDBytes.Length), 0, data, index, sizeof(int));
            index += sizeof(int);

            Array.Copy(ownerUserIDBytes, 0, data, index, ownerUserIDBytes.Length);
            index += ownerUserIDBytes.Length;

            return data;
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
        public AuthenticationLevel AuthenticationLevel;
    }
}
