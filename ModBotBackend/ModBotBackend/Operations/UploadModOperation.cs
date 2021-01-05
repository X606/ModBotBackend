using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HttpUtils;
using ModLibrary;
using ModBotBackend.Users.Sessions;

namespace ModBotBackend.Operations
{
	[Operation("uploadMod")]
	public class UploadModOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			HttpMultipartParser httpMultipartParser = new HttpMultipartParser(context.Request.InputStream, "file");

			if(!httpMultipartParser.Success)
			{
				Utils.RederectToErrorPage(context, "Invalid request");
				return;
			}

			if (!httpMultipartParser.Parameters.ContainsKey("session"))
			{
				Utils.RederectToErrorPage(context, "You're not logged in, you have to be logged in to upload mods");
				return;
			}

			if (!authentication.IsSignedIn)
			{
				Utils.RederectToErrorPage(context, "You are not signed in.");
				return;
			}
			if (!authentication.HasAtLeastAuthenticationLevel(Users.AuthenticationLevel.VerifiedUser))
			{
				Utils.RederectToErrorPage(context, "You have to link a clone drone account to your account to upload mods, this is to prevent people from getting around bans.");
				return;
			}

			string sessionId = authentication.SessionID;

			ModInfo modInfo;
			if (!UploadedModsManager.TryUploadModFromZip(httpMultipartParser.FileContents, sessionId, out modInfo, out string error)) {
				Utils.RederectToErrorPage(context, error);
				return;
			}

			Utils.RederectToErrorPage(context, "Uploaded Mod!", false);
		}

	}
}
