using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using ModBotBackend;
using ModBotBackend.Users.Sessions;

namespace ModBotBackend.Users
{
	public static class UserManager
	{
		public static string ProfilePicturesPath = Program.UsersPath + "/ProfilePictures/";

		public static string DiscordClientSecret;

		public static List<User> Users = new List<User>();

		public static void Init()
		{
			string discordClientSecretPath = Program.DiscordClientSecretPath;

			if(!File.Exists(discordClientSecretPath))
			{
				File.WriteAllText(discordClientSecretPath, "<Your Client Secret Here>");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Please fill out your client secret :)");
				return;
			}

			DiscordClientSecret = File.ReadAllText(discordClientSecretPath);

			LoadUsers();

			Directory.CreateDirectory(ProfilePicturesPath);
		}

		public static void LoadUsers()
		{
			string[] users = Directory.GetFiles(Program.UsersPath);
			foreach(string userFilePath in users)
			{
				Users.Add(User.GetFromFile(userFilePath));
			}
		}
		public static User GetUserFromId(string id)
		{
			foreach(User user in Users)
			{
				if(user.UserID == id)
					return user;
			}

			return null;
		}
		public static User GetUserFromUsername(string username)
		{
			foreach(User user in Users)
			{
				if(user.Username == username)
					return user;
			}

			return null;
		}

		public static Session SignInAsUser(string username, string password)
		{
			User user = GetUserFromUsername(username);

			if(user == null)
				return null;

			if(!user.VeryfyPassword(password))
				return null;

			Session session = new Session(user.UserID);

			return session;
		}
	}
}
