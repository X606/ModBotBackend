using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("getips")]
    public class GetIpsCommand : AdminCommand
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
                    OutputConsole.WriteLine("Unable to find user with id \"" + userId + "\"");
                    return;
                }
            }

            StringBuilder str = new StringBuilder();

            str.Append("Ips used by " + user.Username + ":\n");

            foreach (string ip in user.Ips)
            {
                str.Append(ip + "\n");
            }

            OutputConsole.WriteLine(str.ToString());
        }
    }
}
