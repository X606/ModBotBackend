using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend;
using ModBotBackend.Users;
using ModBotBackend.Users.Sessions;
using HttpUtils;
using System.Net;
using ModLibrary;
using System.IO;

namespace ModBotBackend.Operations
{
	[Operation("uploadProfilePicture")]
	public class UploadProfilePictureOperation : OperationBase
	{
		public override bool ParseAsJson => false;
		public override string[] Arguments => new string[] { };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
		public override bool HideInAPI => true;

		readonly string[] _validFileExtensions = new string[]
			{
				".png",
				".gif",
				".jpg"
			};

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			HttpMultipartParser httpMultipartParser = new HttpMultipartParser(context.Request.InputStream, "file");

			if(!httpMultipartParser.Success)
			{
				Utils.RederectToErrorPage(context, "Invalid request");
				return;
			}

			if(!authentication.IsSignedIn)
			{
				Utils.RederectToErrorPage(context, "You are not signed in.");
				return;
			}

			string extension = Path.GetExtension(httpMultipartParser.Filename);
			byte[] imageData = httpMultipartParser.FileContents;

			if (!_validFileExtensions.Contains(extension.ToLower()))
			{
				Utils.RederectToErrorPage(context, string.Format("The format \"{0}\" is not supported", extension));
				return;
			}

			string path = UserManager.Instance.ProfilePicturesPath + authentication.UserID;

			foreach(string ext in _validFileExtensions)
			{
				string ph = path + ext;
				if (File.Exists(ph))
				{
					File.Delete(ph);
				}
			}

			File.WriteAllBytes(path + extension, imageData);
			Utils.RederectToErrorPage(context, "Uploaded profile picture!", false);
		}
	}
}
