﻿using ModBotBackend.Managers;
using ModBotBackend.Users;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("setdownload")]
    public class SetModBotDownloadLinkCommand : AdminCommand
    {

        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            if (arguments.Length == 0)
            {
                OutputConsole.WriteLine("the setdownload command takes at least 1 argument");
                return;
            }

            string download = getAfterAsString(arguments, 0);

            ModBotInfoManager.Instance.SetModBotDownloadLink(download);
            OutputConsole.WriteLine("Mod bot download link updated to \"<a href=\"" + download + "\">" + download + "</a>\"", true);
        }
    }

}
