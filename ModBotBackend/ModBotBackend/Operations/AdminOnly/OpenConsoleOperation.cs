using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
    [Operation("console")]
    public class OpenConsoleOperation : PlainTextOperationBase
    {
        public override bool ParseAsJson => false;
        public override string[] Arguments => new string[] { };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.Admin;
        public override string OverrideAPICallJavascript => "window.open(\"/api/?operation=console\");";


        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            ContentType = "text/html";

            if (!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.Admin))
            {
                return Properties.Resources.ConsoleCantAccess;
            }

            string html = Properties.Resources.Console;

            string css = ConsoleCustomCssManager.Instance.GetCssForUserID(authentication.UserID);
            if (css == null)
                css = Properties.Resources.ConsoleDefaultCss;

            html = Utils.FormatString(html, css);

            return html;
        }
    }
}
