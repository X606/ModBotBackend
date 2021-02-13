using ModBotBackend.Users;
using ModBotBackend.Users.Sessions;
using System.Text;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("getsessions")]
    public class GetSessionsCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Sessions:\n");
            for (int i = 0; i < SessionsManager.Instance.Sessions.Count; i++)
            {
                Session session = SessionsManager.Instance.Sessions[i];

                stringBuilder.Append(session.OwnerUserID);
                stringBuilder.Append(" (");
                stringBuilder.Append(UserManager.Instance.GetUserFromId(session.OwnerUserID).Username);
                stringBuilder.Append("): ");
                stringBuilder.Append("Expiery time: ");
                stringBuilder.Append(session.ExpieryTime.ToString());
                stringBuilder.Append("\n");

            }

            OutputConsole.WriteLine(stringBuilder.ToString());
        }
    }
}
