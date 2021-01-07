using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
