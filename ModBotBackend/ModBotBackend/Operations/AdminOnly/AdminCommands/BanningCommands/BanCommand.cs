using ModBotBackend.Managers;
using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("ban")]
    public class BanCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            if (arguments.Length == 0 || string.IsNullOrWhiteSpace(arguments[0]))
            {
                OutputConsole.WriteLine("Invalid arguments, you did not provide a argument");
                return;
            }

            string userId = arguments[0];

            User user = UserManager.Instance.GetUserFromId(userId);

            if (user == null)
            {
                OutputConsole.WriteLine("Unable to find user with the id \"" + userId + "\"");
                return;
            }

            BannedUsersManager.Instance.BanUser(user.UserID);

            OutputConsole.WriteLine("Banned " + user.Username + ".");
        }
    }
}
