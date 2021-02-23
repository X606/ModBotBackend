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
        public void BanIp(string ip)
        {
            HashSet<string> bannedIps = _database.Get<HashSet<string>>(BANNED_IPS_NAME);
            if (bannedIps == null)
                bannedIps = new HashSet<string>();

            bannedIps.Add(ip);

            _database.Set(BANNED_IPS_NAME, bannedIps);
        }
        public void HardBanIp(string ip)
        {
            HashSet<string> hardBannedIps = _database.Get<HashSet<string>>(HARD_BANNED_IPS_NAME);
            if (hardBannedIps == null)
                hardBannedIps = new HashSet<string>();

            hardBannedIps.Add(ip);

            _database.Set(HARD_BANNED_IPS_NAME, hardBannedIps);
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
        public void UnbanIp(string ip)
        {
            HashSet<string> bannedIps = _database.Get<HashSet<string>>(BANNED_IPS_NAME);
            HashSet<string> hardBannedIps = _database.Get<HashSet<string>>(HARD_BANNED_IPS_NAME);

            if (bannedIps != null)
            {
                if (bannedIps.Contains(ip))
                {
                    bannedIps.Remove(ip);
                    _database.Set(BANNED_IPS_NAME, bannedIps);
                }
            }
            if (hardBannedIps != null)
            {
                if (hardBannedIps.Contains(ip))
                {
                    hardBannedIps.Remove(ip);
                    _database.Set(HARD_BANNED_IPS_NAME, hardBannedIps);
                }
            }
        }

        public bool IsUserBanned(string userId)
        {
            HashSet<string> bannedUsers = _database.Get<HashSet<string>>(BANNED_USERS_NAME);
            if (bannedUsers == null)
                return false;

            return bannedUsers.Contains(userId);
        }
        public bool IsIpBanned(string ip)
        {
            HashSet<string> bannedIps = _database.Get<HashSet<string>>(BANNED_IPS_NAME);
            if (bannedIps == null)
                return false;

            return bannedIps.Contains(ip);
        }
        public bool IsIpHardBanned(string ip)
        {
            HashSet<string> hardBannedIps = _database.Get<HashSet<string>>(HARD_BANNED_IPS_NAME);
            if (hardBannedIps == null)
                return false;

            return hardBannedIps.Contains(ip);
        }


    }
}
