using ModBotBackend.Users;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("getuserid")]
    public class GetUserIdCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            User user = UserManager.Instance.GetUserFromUsername(arguments[0]);
            if (user == null)
            {
                OutputConsole.WriteLine("The provided user, \"" + arguments[0] + "\" doesnt exist");
                return;
            }
            OutputConsole.WriteLine(arguments[0] + "s userId is: " + user.UserID);
        }
    }
}
