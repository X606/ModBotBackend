using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands.AccountTags
{
    [AdminCommand("removeaccounttag")]
    public class RemoveAccountTagCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            if (arguments.Length < 2)
            {
                OutputConsole.WriteLine("Usage: removeaccounttag <userid> <tag>");
                return;
            }

            string userId = arguments[0];
            string tag = arguments[1];

            User user = UserManager.Instance.GetUserFromId(userId);
            if (user == null)
            {
                OutputConsole.WriteLine("Could not find user with it \"" + userId + "\"");
                return;
            }

            bool removed = user.AccoutTags.Remove(tag);

            if (removed)
            {
                user.Save();
                OutputConsole.WriteLine("Removed tag \"" + tag + "\" from " + user.Username + ".");
            }
            else
            {
                OutputConsole.WriteLine(user.Username + " doesnt have a \"" + tag + "\" tag.");
            }

        }
    }
}
