﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using System.IO;
using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
	[Operation("downloadTempFile")]
	public class DownloadTempFileOperation : OperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "key" };
		public override string OverrideAPICallJavascript => "window.open(\"/api/?operation=downloadTempFile&key=\" + key);";
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string key = context.Request.QueryString["key"];
			
			if (!TemporaryFilesMananger.Instance.TryGetTempFile(key, out byte[] data, out string filename))
			{
				Utils.RederectToErrorPage(context, "The requested file does not exist");
				return;
			}
			
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.SendFile(data, filename.Replace(" ", "_"));
			httpStream.Close();
		}

	}
}
