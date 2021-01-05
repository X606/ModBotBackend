using HttpUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend.Users;
namespace ModBotBackend.Operations.AdminOnly
{
	[Operation("adminCommand")]
	public class AdminCommandOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{

			if (!authentication.HasAtLeastAuthenticationLevel(Users.AuthenticationLevel.Admin))
			{
				context.Response.ContentType = "text/plain";
				HttpStream httapStream = new HttpStream(context.Response);
				httapStream.Send("Access denied :/");
				httapStream.Close();
				return;
			}

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			Request request = JsonConvert.DeserializeObject<Request>(json);

			string message = request.Message;
			if (message.Length > 512)
			{
				context.Response.ContentType = "text/plain";
				HttpStream httapStream = new HttpStream(context.Response);
				httapStream.Send("This command is too long :/");
				httapStream.Close();
				return;
			}

			executeAdminCommand(message.Split(' '), authentication);

			context.Response.ContentType = "text/plain";
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send("Ran command.");
			httpStream.Close();
		}

		static void executeAdminCommand(string[] subStrings, Authentication authentication)
		{
			User user = UserManager.GetUserFromId(authentication.UserID);
			string baseCommand = subStrings[0];

			OutputConsole.WriteLine(user.Username + ": " + getAfterAsString(subStrings, 0));

			if (baseCommand == "print")
			{
				OutputConsole.WriteLine(getAfterAsString(subStrings, 1));
			} else if(baseCommand == "say")
			{
				OutputConsole.WriteLine(user.Username + ": " + getAfterAsString(subStrings, 1));
			} else if(baseCommand == "setUserData")
			{
				if (subStrings.Length < 4)
				{
					OutputConsole.WriteLine("The setUserData command takes 3 arguments: [targetUserID] [targetData] [newData]");
					return;
				}

				string targetUserId = subStrings[1];
				string targetData = subStrings[2].ToLower();
				string newData = getAfterAsString(subStrings, 3);

				User targetUser = UserManager.GetUserFromId(targetUserId);

				if (targetUser == null)
				{
					OutputConsole.WriteLine("The target user could not be found");
					return;
				}

				if (user.AuthenticationLevel <= targetUser.AuthenticationLevel && user != targetUser)
				{
					OutputConsole.WriteLine("You must have a higher authentication level than the target to preform this action");
					return;
				}

				if (targetData == "authenticationlevel")
				{
					AuthenticationLevel authenticationLevel;
					if (int.TryParse(newData, out int authLevel))
					{
						authenticationLevel = (AuthenticationLevel)authLevel;
					}
					else
					{
						OutputConsole.WriteLine("The provided new data was not a valid number");
						return;
					}
					targetUser.AuthenticationLevel = authenticationLevel;
					targetUser.Save();
					OutputConsole.WriteLine("Updated " + targetUser.Username + "s AuthenticationLevel to " + authenticationLevel);
				} else if(targetData == "bio")
				{
					targetUser.Bio = newData;
					targetUser.Save();
					OutputConsole.WriteLine("Updated " + targetUser.Username + "s bio to " + newData);
				} else if(targetData == "color")
				{
					targetUser.DisplayColor = newData;
					targetUser.Save();
					OutputConsole.WriteLine("Updated " + targetUser.Username + "s color to " + newData);
				}
				else if (targetData == "username")
				{
					targetUser.Username = newData;
					targetUser.Save();
					OutputConsole.WriteLine("Updated " + targetUser.Username + "s username to " + newData);
				} 
				else if (targetData == "password")
				{
					targetUser.SetPassword(newData);
					OutputConsole.WriteLine("Updated " + targetUser.Username + " password.");
				}
				else
				{
					OutputConsole.WriteLine("The provided targetData was invalid");
				}

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
