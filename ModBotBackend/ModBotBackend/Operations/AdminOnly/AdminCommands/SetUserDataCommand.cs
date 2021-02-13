using ModBotBackend.Users;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("setUserData")]
    public class SetUserDataCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User user)
        {
            if (arguments.Length < 3)
            {
                OutputConsole.WriteLine("The setUserData command takes 3 arguments: [targetUserID] [targetData] [newData]");
                return;
            }

            string targetUserId = arguments[0];
            string targetData = arguments[1].ToLower();
            string newData = getAfterAsString(arguments, 2);

            User targetUser = UserManager.Instance.GetUserFromId(targetUserId);

            if (targetUser == null)
            {
                OutputConsole.WriteLine("The target user could not be found");
                return;
            }

            if (user.AuthenticationLevel <= targetUser.AuthenticationLevel && user != targetUser)
            {
                OutputConsole.WriteLine("You must have a higher authentication level than the target to preform this action");
                return;
            }

            if (targetData == "authenticationlevel")
            {
                AuthenticationLevel authenticationLevel;
                if (int.TryParse(newData, out int authLevel))
                {
                    authenticationLevel = (AuthenticationLevel)authLevel;
                }
                else
                {
                    OutputConsole.WriteLine("The provided new data was not a valid number");
                    return;
                }
                targetUser.AuthenticationLevel = authenticationLevel;
                targetUser.Save();
                OutputConsole.WriteLine("Updated " + targetUser.Username + "s AuthenticationLevel to " + authenticationLevel);
            }
            else if (targetData == "bio")
            {
                targetUser.Bio = newData;
                targetUser.Save();
                OutputConsole.WriteLine("Updated " + targetUser.Username + "s bio to " + newData);
            }
            else if (targetData == "color")
            {
                targetUser.DisplayColor = newData;
                targetUser.Save();
                OutputConsole.WriteLine("Updated " + targetUser.Username + "s color to " + newData);
            }
            else if (targetData == "username")
            {
                targetUser.Username = newData;
                targetUser.Save();
                OutputConsole.WriteLine("Updated " + targetUser.Username + "s username to " + newData);
            }
            else if (targetData == "password")
            {
                targetUser.SetPassword(newData);
                OutputConsole.WriteLine("Updated " + targetUser.Username + " password.");
            }
            else
            {
                OutputConsole.WriteLine("The provided targetData was invalid");
            }
        }
    }
}
