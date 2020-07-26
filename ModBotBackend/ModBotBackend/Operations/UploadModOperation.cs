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
	public class UploadModOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context)
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

			string sessionId = httpMultipartParser.Parameters["session"];

			Session session;
			if (!SessionsManager.VerifyKey(sessionId, out session))
			{
				Utils.RederectToErrorPage(context, "The session id provided is outdated or invalid");
				return;
			}

			ModInfo modInfo;
			if (!UploadedModsManager.TryUploadModFromZip(httpMultipartParser.FileContents, sessionId, out modInfo, out string error)) {
				Utils.RederectToErrorPage(context, error);
				return;
			}

			Utils.RederectToErrorPage(context, "Uploaded Mod!", false);
		}

	}
}
