using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
	[Operation("getUserHeader")]
	public class GetUserHeaderOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "text/html";
			string html = Encoding.UTF8.GetString(WebsiteRequestProcessor.OnRequest("/userHeader.html", out string contentType));

			string userID = context.Request.QueryString["userID"];
			if (userID == null)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(Utils.FormatString(html, "404.html", "Assets/DefaultAvatar.png", "[Deleted user]", "", ""));
				stream.Close();
				return;
			}

			User user = UserManager.GetUserFromId(userID);
			if (user == null)
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send(Utils.FormatString(html, "404.html", "Assets/DefaultAvatar.png", "[Deleted user]", "", ""));
				stream.Close();
				return;
			}

			string linkUrl = "userPage.html?userID=" + user.UserID;
			string avatarUrl = GetAvatarUrl(user, out string avatarStyle);
			string username = user.Username;
			string icon = GetIconHtml(user);


			string formatedString = Utils.FormatString(html, linkUrl, avatarUrl, username, icon, avatarStyle);

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(formatedString);
			httpStream.Close();
			return;
		}

		static string GetIconHtml(User user)
		{
			switch (user.AuthenticationLevel)
			{
				case AuthenticationLevel.Admin:
					return "<i id='icon' style='color: var(--tertiaryRed)' title='Admin' class='material-icons'>miscellaneous_services</i>";
				case AuthenticationLevel.Modder:
					return "<i id='icon' style='color: var(--tertiaryOrange)' title='Modder' class='material-icons'>construction</i>";
				case AuthenticationLevel.BasicUser:
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
			} else
			{
				avatarStyle = "";
			}

			return "/api/?operation=getProfilePicture&id=" + user.UserID;
		}

	}
}
