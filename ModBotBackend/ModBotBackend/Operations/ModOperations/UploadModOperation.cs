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
using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
	[Operation("uploadMod")]
	public class UploadModOperation : OperationBase
	{
		public override bool ParseAsJson => false;
		public override string[] Arguments => new string[] { };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
		public override bool HideInAPI => true;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			HttpMultipartParser httpMultipartParser = new HttpMultipartParser(context.Request.InputStream, "file");

			if(!httpMultipartParser.Success)
			{
				Utils.SendErrorPage(context.Response, "Invalid request", true, HttpStatusCode.BadRequest);
				return;
			}

			if (!authentication.IsSignedIn)
			{
				Utils.SendErrorPage(context.Response, "You are not signed in.", true, HttpStatusCode.Unauthorized);
				return;
			}
			if (!authentication.HasAtLeastAuthenticationLevel(Users.AuthenticationLevel.VerifiedUser))
			{
				Utils.SendErrorPage(context.Response, "You have to link a clone drone account to your account to upload mods, this is to prevent people from getting around bans.", true, HttpStatusCode.Unauthorized);
				return;
			}

			string sessionId = authentication.SessionID;

			ModInfo modInfo;
			if (!UploadedModsManager.Instance.TryUploadModFromZip(httpMultipartParser.FileContents, sessionId, out modInfo, out string error)) {
				Utils.SendErrorPage(context.Response, error, true, HttpStatusCode.InternalServerError);
				return;
			}

			Utils.SendErrorPage(context.Response, "Uploaded Mod!", false, HttpStatusCode.OK);
		}

	}
}
