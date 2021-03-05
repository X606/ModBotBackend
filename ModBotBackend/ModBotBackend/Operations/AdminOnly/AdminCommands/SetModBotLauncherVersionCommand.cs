using ModBotBackend.Managers;
using ModBotBackend.Users;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("setlauncherversion")]
    public class SetModBotLauncherVersionCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            if (arguments.Length == 0)
            {
                OutputConsole.WriteLine("the setlauncherversion command takes at least 1 argument");
                return;
            }

            string version = getAfterAsString(arguments, 0);

            ModBotInfoManager.Instance.SetModBotLauncherVersion(version);
            OutputConsole.WriteLine("Mod bot Launcher version updated to \"" + version + "\"");
        }
    }
}
