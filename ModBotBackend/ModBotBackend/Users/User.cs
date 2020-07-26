using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.IO;

namespace ModBotBackend.Users
{
	[Serializable]
	public class User
	{
		private User() { }

		public string Username;
		public string PasswordHash;

		public string ProfilePicture;
		public string Bio;

		public string UserID;

		public List<string> FollowedUsers = new List<string>();
		public List<string> FavoritedMods = new List<string>();
		public List<string> LikedMods = new List<string>();
		public List<UserReport> Reports = new List<UserReport>();

		public void SetPassword(string password)
		{
			byte[] salt;
			new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
			byte[] hash = pbkdf2.GetBytes(20);

			byte[] hashBytes = new byte[36];
			Array.Copy(salt, 0, hashBytes, 0, 16);
			Array.Copy(hash, 0, hashBytes, 16, 20);

			PasswordHash = Convert.ToBase64String(hashBytes);

			Save();
		}
		public bool VeryfyPassword(string password)
		{
			byte[] hashBytes = Convert.FromBase64String(PasswordHash);

			byte[] salt = new byte[16];
			Array.Copy(hashBytes, 0, salt, 0, 16);

			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
			byte[] hash = pbkdf2.GetBytes(20);
			/* Compare the results */
			for(int i = 0; i < 20; i++)
				if(hashBytes[i+16] != hash[i])
					return false;

			return true;
		}

		public void Save()
		{
			string path = Program.UsersPath + UserID + ".json";

			string json = JsonConvert.SerializeObject(this);

			File.WriteAllText(path, json);
		}
		public static User GetFromFile(string path)
		{
			string json = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<User>(json);
		}

		public static User CreateNewUser(string username, string password)
		{
			User user = new User();
			user.Username = username;
			user.UserID = Guid.NewGuid().ToString();
			user.Bio = "";
			user.SetPassword(password);

			UserManager.Users.Add(user);
			return user;
		}
	}
}
