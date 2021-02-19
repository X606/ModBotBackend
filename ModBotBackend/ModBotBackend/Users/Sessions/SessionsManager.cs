using System;
using System.Collections.Generic;
using System.IO;

namespace ModBotBackend.Users.Sessions
{
    [FolderName("Sessions")]
    public class SessionsManager : OwnFolderObject<SessionsManager>
    {
        public const float SESSION_LENGTH = 365.25f;

        public const string SESSIONS_SAVE_FILE_NAME = "SessionsData.data";
        public List<Session> Sessions = new List<Session>();

        public override void OnStartup()
        {
            string filePath = GetPathForFile(SESSIONS_SAVE_FILE_NAME);
            Console.WriteLine(filePath);

            if (File.Exists(filePath))
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    byte[] intBuffer = new byte[sizeof(int)];
                    stream.Read(intBuffer, 0, sizeof(int));
                    int sessionsCount = BitConverter.ToInt32(intBuffer, 0);

                    for (int i = 0; i < sessionsCount; i++)
                    {
                        stream.Read(intBuffer, 0, sizeof(int));

                        byte[] data = new byte[BitConverter.ToInt32(intBuffer, 0)];
                        stream.Read(data, 0, data.Length);

                        new Session(data);
                    }

                }


            }
        }
        public override void OnShutDown()
        {
            SaveToFile();

        }

        public void SaveToFile()
        {
            RemoveAllExpierdSessions();

            string filePath = GetPathForFile(SESSIONS_SAVE_FILE_NAME);

            using (FileStream stream = File.Create(filePath))
            {
                stream.Write(BitConverter.GetBytes(Sessions.Count), 0, sizeof(int));
                for (int i = 0; i < Sessions.Count; i++)
                {
                    byte[] data = Sessions[i].ToData();
                    stream.Write(BitConverter.GetBytes(data.Length), 0, sizeof(int));
                    stream.Write(data, 0, data.Length);
                }


            }
        }

        public void RemoveAllExpierdSessions()
        {
            for (int i = Sessions.Count - 1; i >= 0; i--)
            {
                if (Sessions[i].IsExpierd())
                {
                    Sessions.RemoveAt(i);
                }
            }

        }

        public void RemoveSession(string key)
        {
            for (int i = Sessions.Count - 1; i >= 0; i--)
            {
                if (Sessions[i].Key == key)
                {
                    Sessions.RemoveAt(i);
                    return;
                }
            }
        }

        public bool VerifyKey(string key, out Session session)
        {
            foreach (Session selectedSession in Sessions)
            {
                if (selectedSession.Key == key && !selectedSession.IsExpierd())
                {
                    session = selectedSession;
                    return true;
                }
            }

            session = null;
            return false;
        }

        public string GetPlayerIdFromSession(string sessionKey)
        {
            foreach (Session session in Sessions)
            {
                if (session.Key == sessionKey)
                    return session.OwnerUserID;

            }

            return null;
        }

        public void OnUserInfoUpdated(User user)
        {
            foreach (Session session in Sessions)
            {
                if (session.OwnerUserID == user.UserID)
                    session.AuthenticationLevel = user.AuthenticationLevel;

            }
        }
    }
}
