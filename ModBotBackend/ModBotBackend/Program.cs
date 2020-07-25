using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net;
using System.IO;
using ModBotBackend.Operations;

namespace ModBotBackend
{
	static class Program
	{
		static void Main(string[] args)
		{
			UploadedModsManager.Setup(DataPath);

			HttpListener httpListener = new HttpListener();
			httpListener.Prefixes.Add("http://*:80/");
			httpListener.Start();
			listen(httpListener);


			Console.WriteLine("Listening...");
			Console.WriteLine("Press any key to exit...");
			Console.ReadLine();
		}

		static async void listen(HttpListener httpListener)
		{
			while(true)
			{
				var context = await httpListener.GetContextAsync();
				//Console.WriteLine("Client connected");
				Task.Factory.StartNew(() => processRequest(context));
			}
		}

		static void processRequest(HttpListenerContext context)
		{
			HttpListenerRequest request = context.Request;

			string operation = request.QueryString.Get("operation");
			
			string absolutePath = request.Url.AbsolutePath;
			if(absolutePath == "/api/" || absolutePath == "/api")
			{
				if(operation != null && Operations.TryGetValue(operation, out OperationBase selectedOperation))
				{
					try
					{
						selectedOperation.OnOperation(context);
					}
					catch
					{
						context.Response.Redirect("https://clonedronemodbot.com/error.html?error=an error occured");
						HttpStream httpStream = new HttpStream(context.Response);
						httpStream.Send("re-routed");
						httpStream.Close();
					}
				}
				else
				{
					if(operation == null)
						operation = "null";

					context.Response.Redirect("https://clonedronemodbot.com/error.html?error=invalid operation \"" + operation + "\"");
					HttpStream httpStream = new HttpStream(context.Response);
					httpStream.Send("re-routed");
					httpStream.Close();
				}
			} else
			{
				if (absolutePath == "" || absolutePath == "/")
				{
					absolutePath = "/index.html";
				}

				string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Website" + absolutePath;
				
				if(File.Exists(path))
				{
					byte[] data = File.ReadAllBytes(path);

					context.Response.ContentLength64 = data.Length;
					context.Response.OutputStream.Write(data, 0, data.Length);
					context.Response.OutputStream.Close();
				} else
				{
					string pageNotFoundPage = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Website/404.html";
					if (File.Exists(pageNotFoundPage))
					{
						context.Response.Redirect("404.html");
						HttpStream httpStream = new HttpStream(context.Response);
						httpStream.Send("re-routed");
						httpStream.Close();
					} else
					{
						HttpStream httpStream = new HttpStream(context.Response);
						httpStream.Send("404 :(");
						httpStream.Close();
					}
					
				}
				
			}
		}

		public static string DataPath => Directory.GetCurrentDirectory() + "/Data/";

		public static readonly Dictionary<string, OperationBase> Operations = new Dictionary<string, OperationBase>()
		{
			{ "test", new TestOperation() },
			{ "img", new ImageOperation() },
			{ "post", new PostOperation() }
		};
		

	}
}
