using ModBotBackend.Managers;
using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("hardbanip")]
    public class HardIpBanCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            if (arguments.Length == 0 || string.IsNullOrWhiteSpace(arguments[0]))
            {
                OutputConsole.WriteLine("Invalid arguments, you did not provide a argument");
                return;
            }

            string ip = arguments[0];

            BannedUsersManager.Instance.HardBanIp(ip);
            OutputConsole.WriteLine("Hard banned \"" + ip + "\"");
        }
    }
}
