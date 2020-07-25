using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HttpUtils;

namespace ModBotBackend.Operations
{
	public class PostOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context)
		{
			HttpMultipartParser httpMultipartParser = new HttpMultipartParser(context.Request.InputStream, "file");
			
			if (!httpMultipartParser.Success)
				throw new Exception("Something went wrong");

			string text = httpMultipartParser.Parameters["email"];

			HttpStream httpStream = new HttpStream(context.Response);	
			httpStream.Send("<head></head><body><p>" + text +"</p></body>");
			httpStream.Close();
		}

	}
}
