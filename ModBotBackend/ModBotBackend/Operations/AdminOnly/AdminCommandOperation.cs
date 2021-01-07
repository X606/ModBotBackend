using HttpUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend.Users;
using ModBotBackend.Operations.AdminOnly.AdminCommands;

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

			executeAdminCommand(message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), authentication);

			context.Response.ContentType = "text/plain";
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send("Ran command.");
			httpStream.Close();
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