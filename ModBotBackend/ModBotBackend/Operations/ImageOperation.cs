using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModBotBackend.Operations
{
	public class ImageOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context)
		{
			string img = context.Request.QueryString.Get("img");

			string path = Program.DataPath + img;
			if(!File.Exists(path))
			{
				HttpStream stream = new HttpStream(context.Response);
				stream.Send("sorry, img doesnt exist");
				stream.Close();
				return;
			}

			byte[] fileData = File.ReadAllBytes(path);
			HttpListenerResponse response = context.Response;
			
			// Get a response stream and write the response to it.
			response.ContentLength64 = fileData.Length;
			response.OutputStream.Write(fileData, 0, fileData.Length);
			response.OutputStream.Close();
		}
	}
}
