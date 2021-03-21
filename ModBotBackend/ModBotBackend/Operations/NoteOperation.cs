using ModBotBackend.Managers;
using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations
{
    [Operation("noteOperation")]
    public class NoteOperation : PlainTextOperationBase
    {
        public override string[] Arguments => new string[] { "key" };

        public override bool ParseAsJson => false;

        public override bool ArgumentsInQuerystring => true;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            string key = arguments["key"];

            if (key == null)
            {
                return "[Invalid key]";
            }

            string note = NotesManager.Instance.GetNote(key);

            if (note == null)
            {
                return "[Invalid note]";
            }

            return note;
        }
    }
}
