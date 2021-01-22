using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend.Managers;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
	[AdminCommand("setdownload")]
	public class SetModBotDownloadLinkCommand : AdminCommand
	{

		public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
		{
			if (arguments.Length == 0)
			{
				OutputConsole.WriteLine("the setdownload command takes at least 1 argument");
				return;
			}

			string download = getAfterAsString(arguments, 0);

			ModBotInfoManager.Instance.SetModBotDownloadLink(download);
			OutputConsole.WriteLine("Mod bot download link updated to \"<a href=\"" + download + "\">" + download + "</a>\"", true);
		}
	}

}
