using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net;
using System.IO;
namespace ModBotBackend
{
	public class HttpStream
	{
		HttpListenerResponse _response;
		Stream _output;

		public HttpStream(HttpListenerResponse response)
		{
			_response = response;
			_output = _response.OutputStream;
		}

		public void Send(string responseString)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(responseString);
			// Get a response stream and write the response to it.
			_response.ContentLength64 += buffer.Length;
			_output.Write(buffer, 0, buffer.Length);

		}
		public void Close()
		{
			_output.Close();
		}

	}
}
