using ModBotBackend.Users;
using System.Text;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("DisplayAllUsers")]
    public class DisplayAllUsersCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Signed up Users:\n");
            for (int i = 0; i < UserManager.Instance.Users.Count; i++)
            {
                User user = UserManager.Instance.Users[i];
                builder.Append(user.Username + " (UserId: " + user.UserID + ")");

                if (i != (UserManager.Instance.Users.Count - 1))
                    builder.Append("\n");
            }
            OutputConsole.WriteLine(builder.ToString());

        }
    }
}
