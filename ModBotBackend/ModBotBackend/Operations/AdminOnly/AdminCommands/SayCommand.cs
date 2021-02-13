using ModBotBackend.Users;
namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("say")]
    public class SayCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User user)
        {
            OutputConsole.WriteLine(user.Username + ": " + getAfterAsString(arguments, 0));
        }
    }
}
