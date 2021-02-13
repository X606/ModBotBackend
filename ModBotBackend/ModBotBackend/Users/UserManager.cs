using ModBotBackend.Users.Sessions;
using System.Collections.Generic;
using System.IO;

namespace ModBotBackend.Users
{
    [FolderName("Users")]
    public class UserManager : OwnFolderObject<UserManager>
    {
        public string ProfilePicturesPath => FolderPath + "/ProfilePictures/";

        public List<User> Users = new List<User>();

        public override void OnStartup()
        {
            LoadUsers();

            Directory.CreateDirectory(ProfilePicturesPath);
        }

        public void LoadUsers()
        {
            string[] users = Directory.GetFiles(FolderPath);
            foreach (string userFilePath in users)
            {
                Users.Add(User.GetFromFile(userFilePath));
            }
        }
        public User GetUserFromId(string id)
        {
            foreach (User user in Users)
            {
                if (user.UserID == id)
                    return user;
            }

            return null;
        }
        public User GetUserFromUsername(string username)
        {
            foreach (User user in Users)
            {
                if (user.Username == username)
                    return user;
            }

            return null;
        }
        public User GetUserFromPlayfabID(string playfabID)
        {
            foreach (User user in Users)
            {
                if (user.PlayfabID == playfabID)
                    return user;
            }

            return null;
        }

        public Session SignInAsUser(string username, string password)
        {
            User user = GetUserFromUsername(username);

            if (user == null)
                return null;

            if (!user.VeryfyPassword(password))
                return null;

            Session session = new Session(user.UserID, SessionsManager.SESSION_LENGTH);

            return session;
        }


    }
}
