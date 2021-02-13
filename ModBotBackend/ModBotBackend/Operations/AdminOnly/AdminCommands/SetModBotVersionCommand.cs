using ModBotBackend.Managers;
using ModBotBackend.Users;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("setversion")]
    public class SetModBotVersionCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            if (arguments.Length == 0)
            {
                OutputConsole.WriteLine("the setVersion command takes at least 1 argument");
                return;
            }

            string version = getAfterAsString(arguments, 0);

            ModBotInfoManager.Instance.SetModBotVersion(version);
            OutputConsole.WriteLine("Mod bot version updated to \"" + version + "\"");
        }
    }
}
