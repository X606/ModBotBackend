using ModBotBackend.Users;
using ModBotBackend.Users.Sessions;
using System.Collections.Generic;
using System.Text;

namespace ModBotBackend.Operations
{
    [Operation("getAPI")]
    public class GetJavascriptAPIOperation : PlainTextOperationBase
    {
        public override string[] Arguments => new string[] { };
        public override bool ParseAsJson => false;
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override string OnOperation(Arguments passedArguments, Authentication authentication)
        {
            StringBuilder javascriptBuilder = new StringBuilder();

            javascriptBuilder.Append("//AutenticationLevel = " + authentication.AuthenticationLevel.ToString());
            javascriptBuilder.Append("\n\n");

            javascriptBuilder.Append(Utils.FormatString(Properties.Resources.APITemplate, (SessionsManager.SESSION_LENGTH * 24 * 60).ToString()));

            foreach (KeyValuePair<string, OperationBase> item in Program.Operations)
            {
                if (item.Value.HideInAPI)
                    continue;

                if (!authentication.HasAtLeastAuthenticationLevel(item.Value.MinimumAuthenticationLevelToCall))
                    continue;

                javascriptBuilder.Append("API." + item.Key + " = function(");
                string[] arguments = item.Value.Arguments;

                for (int i = 0; i < arguments.Length; i++)
                {
                    javascriptBuilder.Append(arguments[i]);

                    if (i != (arguments.Length - 1))
                        javascriptBuilder.Append(',');
                }

                javascriptBuilder.Append(") { ");
                string overrideJavascript = item.Value.OverrideAPICallJavascript;
                if (overrideJavascript == null)
                {
                    javascriptBuilder.Append(" return new Promise(async resolve => { ");
                    javascriptBuilder.Append("let e = await Post(\"/api?operation=");
                    javascriptBuilder.Append(item.Key);
                    if (item.Value.ArgumentsInQuerystring)
                    {
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            javascriptBuilder.Append('&');


                            javascriptBuilder.Append(arguments[i]);
                            javascriptBuilder.Append("=\" + ");
                            javascriptBuilder.Append(arguments[i]);

                            if (i != (arguments.Length - 1))
                                javascriptBuilder.Append(" + \"");
                        }
                    }
                    else
                    {
                        javascriptBuilder.Append('\"');
                    }

                    javascriptBuilder.Append(",{ ");
                    if (!item.Value.ArgumentsInQuerystring)
                    {
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            javascriptBuilder.Append(arguments[i]);
                            javascriptBuilder.Append(":");
                            javascriptBuilder.Append(arguments[i]);

                            if (i != (arguments.Length - 1))
                                javascriptBuilder.Append(',');
                        }
                    }

                    javascriptBuilder.Append("}); ");

                    if (item.Value.ParseAsJson)
                        javascriptBuilder.Append("e = JSON.parse(e); ");

                    string overrideResolveJavascript = item.Value.OverrideResolveJavascript;
                    if (overrideResolveJavascript == null)
                    {
                        javascriptBuilder.Append("resolve(e); ");
                    }
                    else
                    {
                        javascriptBuilder.Append(overrideResolveJavascript);
                    }


                    javascriptBuilder.Append("}); ");
                }
                else
                {
                    javascriptBuilder.Append(overrideJavascript);
                }
                javascriptBuilder.Append("}; ");
            }

            javascriptBuilder.Append("export { API };");

            ContentType = "application/javascript";

            return javascriptBuilder.ToString();
        }
    }
}
