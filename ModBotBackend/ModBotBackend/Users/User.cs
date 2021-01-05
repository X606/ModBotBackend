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
		
		public string Bio;

		public string UserID;

		public string DisplayColor;
		public BorderStyles BorderStyle;
        public bool ShowFull;

		public AuthenticationLevel AuthenticationLevel = AuthenticationLevel.BasicUser;

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

		const int MIN_USERNAME_LENGTH = 3;
		const int MAX_USERNAME_LENGTH = 40;

		const string ALLOWED_CHARACTERS_IN_USERNAMES = "abcdefghijklmnopqrstuvwxyzåäöABCDEFGIJKLMNOPQRSTUVWXYZÅÄÖ1234567890";
		public static bool IsValidUsername(string username, out string error)
		{
			if(username.Length < MIN_USERNAME_LENGTH)
			{
				error = "invalid length, the username must be at least " + MIN_USERNAME_LENGTH + " characters long";
				return false;
			}
			if(username.Length > MAX_USERNAME_LENGTH)
			{
				error = "invalid length, the username cannot be longer than " + MAX_USERNAME_LENGTH + " characters";
				return false;
			}

			for(int i = 0; i < username.Length; i++)
			{
				if(!ALLOWED_CHARACTERS_IN_USERNAMES.Contains(username[i]))
				{
					error = "the character \"" + username[i] + "\" is not allowed in usernames";
					return false;
				}
			}

			if(UserManager.GetUserFromUsername(username) != null)
			{
				error = "username already taken";
				return false;
			}

			error = string.Empty;
			return true;
		}
		public static bool IsValidPassword(string password, out string error)
		{
			if(password.Length < 4)
			{
				error = "invalid length, the password must be at least 4 characters long";
				return false;
			}
			if(password.Length > 100)
			{
				error = "invalid length, the password is too long, the max limit is 100 characters";
				return false;
			}

			error = string.Empty;
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
			user.BorderStyle = BorderStyles.Runded;
            user.ShowFull = false;
			user.SetPassword(password);
			user.DisplayColor = "#ffffff";

			UserManager.Users.Add(user);
			return user;
		}

		public override bool Equals(object obj) => obj is User user && user == this;

		public override int GetHashCode() => UserID.GetHashCode();
		public static bool operator !=(User a, User b) => !(a == b);

		public static bool operator ==(User a, User b)
		{
			bool aIsNull = ReferenceEquals(a, null);
			bool bIsNull = ReferenceEquals(b, null);
			if (aIsNull || bIsNull)
				return aIsNull && bIsNull;

			return a.UserID == b.UserID;
		}
	}
	public enum BorderStyles
	{
		Square,
		Runded,
		Round
	}

	public enum AuthenticationLevel
	{
		None,
		BasicUser,
		VerifiedUser,
		Modder,
		Admin
	}
}
