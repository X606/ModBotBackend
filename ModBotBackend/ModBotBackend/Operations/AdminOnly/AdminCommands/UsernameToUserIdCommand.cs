using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
	[AdminCommand("usernameToUserId")]
	public class UsernameToUserIdCommand : AdminCommand
	{
		public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
		{
			User user = UserManager.GetUserFromUsername(arguments[0]);
			if (user == null)
			{
				OutputConsole.WriteLine("The provided user, \"" + arguments[0] + "\" doesnt exist");
				return;
			}
			OutputConsole.WriteLine(arguments[0] + "s userId is: " + user.UserID);
		}
	}
}
