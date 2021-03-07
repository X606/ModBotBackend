using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands.AccountTags
{
    [AdminCommand("addaccounttag")]
    public class AddAccountTagCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            if (arguments.Length < 2)
            {
                OutputConsole.WriteLine("Usage: addaccounttag <userid> <tag>");
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

            user.AccoutTags.Add(tag);
            user.Save();
            OutputConsole.WriteLine("Added tag \"" + tag + "\" to " + user.Username + ".");
        }
    }
}
