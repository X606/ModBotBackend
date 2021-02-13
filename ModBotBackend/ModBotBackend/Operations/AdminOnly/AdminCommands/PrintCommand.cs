using ModBotBackend.Users;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("print")]
    public class PrintCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            OutputConsole.WriteLine(getAfterAsString(arguments, 0));
        }
    }
}
