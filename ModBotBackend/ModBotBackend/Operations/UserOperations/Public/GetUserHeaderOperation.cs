using ModBotBackend.Users;
using System.Text;

namespace ModBotBackend.Operations
{
    [Operation("getUserHeader")]
    public class GetUserHeaderOperation : PlainTextOperationBase
    {
        public override bool ParseAsJson => false;
        public override string[] Arguments => new string[] { "element", "userID" };
        public override bool ArgumentsInQuerystring => true;
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
        public override string OverrideAPICallJavascript => "element.contentWindow.location.replace(\"/api?operation=getUserHeader&userID=\" + userID);";

        public override string OnOperation(Arguments arguments, Authentication authentication)
        {
            ContentType = "text/html";
            int statusCode = 200;
            string html = Encoding.UTF8.GetString(WebsiteRequestProcessor.OnRequest("/userHeader.html", out string contentType, ref statusCode));

            string userID = arguments["userID"];
            if (userID == null)
            {
                return Utils.FormatString(html, "404.html", "Assets/DefaultAvatar.png", "[Deleted user]", "", "");
            }

            User user = UserManager.Instance.GetUserFromId(userID);
            if (user == null)
            {
                return Utils.FormatString(html, "404.html", "Assets/DefaultAvatar.png", "[Deleted user]", "", "");
            }

            string linkUrl = "userPage.html?userID=" + user.UserID;
            string avatarUrl = GetAvatarUrl(user, out string avatarStyle);
            string username = user.Username;
            string icon = GetIconHtml(user);

            string usernameStyle = "color: " + user.DisplayColor;

            string formatedString = Utils.FormatString(html, linkUrl, avatarUrl, username, icon, avatarStyle, usernameStyle);

            return formatedString;
        }

        static string GetIconHtml(User user)
        {
            switch (user.AuthenticationLevel)
            {
                case AuthenticationLevel.Admin:
                    return "<i id='icon' style='color: var(--tertiaryRed)' title='Admin' class='material-icons'>miscellaneous_services</i>";
                case AuthenticationLevel.Modder:
                    return "<i id='icon' style='color: var(--tertiaryOrange)' title='Modder' class='material-icons'>construction</i>";
                case AuthenticationLevel.VerifiedUser:
                    return "<i id='icon' style='color: var(--tertiaryBlue)' title='Verified' class='material-icons'>verified</i>";
            }
            return "";
        }

        static string GetAvatarUrl(User user, out string avatarStyle)
        {
            if (user.BorderStyle == BorderStyles.Square)
            {
                avatarStyle = "border-radius: 0px; ";
            }
            else if (user.BorderStyle == BorderStyles.Runded)
            {
                avatarStyle = "border-radius: 25%; ";
            }
            else if (user.BorderStyle == BorderStyles.Round)
            {
                avatarStyle = "border-radius: 50%; ";
            }
            else
            {
                avatarStyle = "";
            }

            return "/api/?operation=getProfilePicture&id=" + user.UserID;
        }

    }
}
