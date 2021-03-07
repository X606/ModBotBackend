using ModBotBackend.Managers;
using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands.AccountTags
{
    [AdminCommand("setrequiredtag")]
    public class SetRequiredAccountTagOnTagCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            if (arguments.Length < 2)
            {
                OutputConsole.WriteLine("usage setrequiredtag <tagid> <account tag>");
                return;
            }

            string tagid = arguments[0];
            string accountTag = arguments[1];

            if (accountTag == "null")
                accountTag = null;

            TagInfo tag = TagsManager.Instance.GetTag(tagid);
            if (tag == null)
            {
                OutputConsole.WriteLine("Could not find tag with the id \"" + tagid + "\"");
                return;
            }

            tag.RequiredAccountTag = accountTag;
            TagsManager.Instance.SaveTag(tag);

            if (accountTag != null)
            {
                OutputConsole.WriteLine("Set the required account tag on the tag with the id \"" + tagid + "\" to \"" + accountTag + "\".");
            } else
            {
                OutputConsole.WriteLine("Removed all required account tags from the tag with the id \"" + tagid + "\".");
            }
        }
    }
}
