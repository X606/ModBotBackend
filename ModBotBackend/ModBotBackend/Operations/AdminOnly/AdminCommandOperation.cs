﻿using ModBotBackend.Operations.AdminOnly.AdminCommands;
using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModBotBackend.Operations.AdminOnly
{
    [Operation("adminCommand")]
    public class AdminCommandOperation : PlainTextOperationBase
    {
        public override bool ParseAsJson => false;
        public override string[] Arguments => new string[] { "Message" };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.Admin;

        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.Admin))
            {
                return "Access denied :/";
            }

            string message = arguments["Message"];
            if (message.Length > 512)
            {
                return "This command is too long :/";
            }

            executeAdminCommand(message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), authentication);

            return "Ran command.";
        }

        static void executeAdminCommand(string[] subStrings, Authentication authentication)
        {

            User user = UserManager.Instance.GetUserFromId(authentication.UserID);
            string baseCommand = subStrings[0].ToLower();

            string[] arguments = new string[subStrings.Length - 1];
            Array.Copy(subStrings, 1, arguments, 0, subStrings.Length - 1);

            OutputConsole.WriteLine(user.Username + ": " + getAfterAsString(subStrings, 0));

            if (_adminCommands == null)
                populateAdminCommands();

            if (baseCommand == "help")
            {
                StringBuilder helpResponseBuilder = new StringBuilder();

                helpResponseBuilder.Append("Commands:\n");

                foreach (KeyValuePair<string, AdminCommand> item in _adminCommands)
                {
                    helpResponseBuilder.Append(item.Key);
                    helpResponseBuilder.Append('\n');
                }

                OutputConsole.WriteLine(helpResponseBuilder.ToString());
                return;
            }

            if (_adminCommands.TryGetValue(baseCommand, out AdminCommand value))
            {
                value.ProcessCommand(arguments, authentication, user);
            }
        }

        static Dictionary<string, AdminCommand> _adminCommands = null;

        static void populateAdminCommands()
        {
            _adminCommands = new Dictionary<string, AdminCommand>();
            Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                AdminCommandAttribute adminCommandAttribute = (AdminCommandAttribute)Attribute.GetCustomAttribute(type, typeof(AdminCommandAttribute));
                if (adminCommandAttribute == null)
                    continue;

                _adminCommands.Add(adminCommandAttribute.CommandKey.ToLower(), (AdminCommand)Activator.CreateInstance(type));
            }

        }

        static string getAfterAsString(string[] array, int index)
        {
            string output = "";
            for (int i = index; i < array.Length; i++)
            {
                output += array[i];

                if (i != (array.Length - 1))
                    output += " ";
            }

            return output;
        }

        [Serializable]
        public class Request
        {
            public string Message;
        }

    }
}
namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    public abstract class AdminCommand
    {
        public abstract void ProcessCommand(string[] arguments, Authentication authentication, User caller);

        protected static string getAfterAsString(string[] array, int index)
        {
            string output = "";
            for (int i = index; i < array.Length; i++)
            {
                output += array[i];

                if (i != (array.Length - 1))
                    output += " ";
            }

            return output;
        }
    }
    public class AdminCommandAttribute : Attribute
    {
        public string CommandKey = "";

        public AdminCommandAttribute(string commandKey)
        {
            CommandKey = commandKey;
        }
    }

}