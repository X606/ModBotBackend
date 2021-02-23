using ModBotBackend.Managers;
using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands.BanningCommands
{
    [AdminCommand("unban")]
    public class UnbanCommand : AdminCommand
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
                user = UserManager.Instance.GetUserFromUsername(userId);
                if (user == null)
                {
                    BannedUsersManager.Instance.UnbanIp(userId);
                    OutputConsole.WriteLine("Tried to unban ip \"" + userId + "\".");
                    return;
                }
            }

            BannedUsersManager.Instance.UnbanUser(userId);
            OutputConsole.WriteLine("Tried to unban user \"" + userId + "\".");
        }
    }
}
