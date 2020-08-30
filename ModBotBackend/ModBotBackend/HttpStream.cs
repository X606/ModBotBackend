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
			byte[] buffer;
			if (responseString != null)
			{
				buffer = Encoding.UTF8.GetBytes(responseString);
			} else
			{
				buffer = Encoding.UTF8.GetBytes("null");
			}
			 
			// Get a response stream and write the response to it.
			_response.ContentLength64 += buffer.Length;
			_output.Write(buffer, 0, buffer.Length);

		}

		public void SendFile(string path)
		{
			using(FileStream fs = File.OpenRead(path))
			{
				string filename = Path.GetFileName(path);
				//response is HttpListenerContext.Response...
				_response.ContentLength64 = fs.Length;
				_response.SendChunked = false;
				_response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
				_response.AddHeader("Content-disposition", "attachment; filename=" + filename);

				byte[] buffer = new byte[64 * 1024];
				int read;
				using(BinaryWriter bw = new BinaryWriter(_response.OutputStream))
				{
					while((read = fs.Read(buffer, 0, buffer.Length)) > 0)
					{
						bw.Write(buffer, 0, read);
						bw.Flush(); //seems to have no effect
					}

					bw.Close();
				}

				_response.StatusCode = (int)HttpStatusCode.OK;
				_response.StatusDescription = "OK";
				_response.OutputStream.Close();
			}
		}
		public void SendFile(byte[] data, string filename)
		{
			//response is HttpListenerContext.Response...
			_response.ContentLength64 = data.Length;
			_response.SendChunked = false;
			_response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
			_response.AddHeader("Content-disposition", "attachment; filename=" + filename);
			_response.OutputStream.Write(data, 0, data.Length);

			/*
			byte[] buffer = new byte[64 * 1024];
			int read;
			using(BinaryWriter bw = new BinaryWriter(_response.OutputStream))
			{
				while((read = fs.Read(buffer, 0, buffer.Length)) > 0)
				{
					bw.Write(buffer, 0, read);
					bw.Flush(); //seems to have no effect
				}

				bw.Close();
			}
			*/

			_response.StatusCode = (int)HttpStatusCode.OK;
			_response.StatusDescription = "OK";
			_response.OutputStream.Close();
		}

		public void Close()
		{
			_response.Close();
		}

	}
}
