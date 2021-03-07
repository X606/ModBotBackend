using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("deleteuser")]
    public class DeleteUserCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            if (arguments.Length == 0)
            {
                OutputConsole.WriteLine("You did not provide a user id to delete");
                return;
            }

            string userid = getAfterAsString(arguments, 0);

            User user = UserManager.Instance.GetUserFromId(userid);
            if (user == null)
            {
                OutputConsole.WriteLine("Could not find user with id \"" + userid + "\"");
                return;
            }

            user.DeleteUser();

            OutputConsole.WriteLine("Deleted user \"" + userid + "\" with the name \"" + user.Username + "\"");
        }
    }
}
