using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
