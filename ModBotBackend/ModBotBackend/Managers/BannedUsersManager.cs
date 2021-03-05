using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend;
using ModBotBackend.Users;

namespace ModBotBackend.Managers
{
    [FolderName("Bans")]
    public class BannedUsersManager : OwnFolderObject<BannedUsersManager>
    {
        public const string BANNED_USERS_NAME = "BannedUsers";
        public const string BANNED_IPS_NAME = "BannedIps";
        public const string HARD_BANNED_IPS_NAME = "HardBannedIps";


        JsonDatabase _database;
        public override void OnStartup()
        {
            _database = new JsonDatabase(GetPathForFile("Bans.json"), JsonDatabase.DatabaseSaveOptions.Automatic);
        }
        public override void OnShutDown()
        {
            _database.Save();
        }

        public void BanUser(string userId)
        {
            HashSet<string> bannedUsers = _database.Get<HashSet<string>>(BANNED_USERS_NAME);
            if (bannedUsers == null)
                bannedUsers = new HashSet<string>();

            bannedUsers.Add(userId);

            _database.Set(BANNED_USERS_NAME, bannedUsers);
        }

        public void UnbanUser(string userId)
        {
            HashSet<string> bannedUsers = _database.Get<HashSet<string>>(BANNED_USERS_NAME);
            if (bannedUsers == null)
                return;

            if (bannedUsers.Contains(userId))
            {
                bannedUsers.Remove(userId);
                _database.Set(BANNED_USERS_NAME, bannedUsers);
            }
        }

        public bool IsUserBanned(string userId)
        {
            HashSet<string> bannedUsers = _database.Get<HashSet<string>>(BANNED_USERS_NAME);
            if (bannedUsers == null)
                return false;

            return bannedUsers.Contains(userId);
        }


    }
}
