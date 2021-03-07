using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands.AccountTags
{
    [AdminCommand("getaccounttags")]
    public class GetAccountTagsCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            if (arguments.Length < 1)
            {
                OutputConsole.WriteLine("Usage: getaccounttags <userid>");
                return;
            }

            string userId = arguments[0];

            User user = UserManager.Instance.GetUserFromId(userId);
            if (user == null)
            {
                OutputConsole.WriteLine("Could not find user with it \"" + userId + "\"");
                return;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Tags on " + user.Username + ": ");
            for (int i = 0; i < user.AccoutTags.Count; i++)
            {
                stringBuilder.Append(user.AccoutTags[i]);

                if (i != (user.AccoutTags.Count - 1))
                    stringBuilder.Append(", ");
            }
            stringBuilder.Append(".");

            OutputConsole.WriteLine(stringBuilder.ToString());
        }
    }
}
