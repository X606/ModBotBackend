using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using System.IO;

namespace ModBotBackend.Operations
{
	public class DownloadTempFileOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string key = context.Request.QueryString["key"];
			
			if (!TemporaryFilesMananger.TryGetTempFile(key, out byte[] data, out string filename))
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
