using ModBotBackend.Managers;
using ModBotBackend.Users;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("setlauncherdownload")]
    public class SetModBotLauncherDownloadLinkCommand : AdminCommand
    {

        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            if (arguments.Length == 0)
            {
                OutputConsole.WriteLine("the setlauncherdownload command takes at least 1 argument");
                return;
            }

            string download = getAfterAsString(arguments, 0);

            ModBotInfoManager.Instance.SetModBotLauncherDownloadLink(download);
            OutputConsole.WriteLine("Mod bot launcher download link updated to \"<a href=\"" + download + "\">" + download + "</a>\"", true);
        }
    }
}
