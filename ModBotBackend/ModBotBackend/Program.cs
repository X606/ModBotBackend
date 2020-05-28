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
		public static UploadedModsManager UploadedModsManager;

		static void Main(string[] args)
		{
			UploadedModsManager = new UploadedModsManager(DataPath);

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
				Console.WriteLine("Client connected");
				Task.Factory.StartNew(() => processRequest(context));
			}
		}

		static void processRequest(HttpListenerContext context)
		{
			HttpListenerRequest request = context.Request;

			string operation = request.QueryString.Get("operation");

			if (operation != null && Operations.TryGetValue(operation, out OperationBase selectedOperation))
			{
				selectedOperation.OnOperation(context);
			}
			else
			{
				if (operation == null)
					operation = "null";

				context.Response.Redirect("https://clonedronemodbot.com/error.html?error=invalid operation \"" + operation + "\"");
				HttpStream httpStream = new HttpStream(context.Response);
				httpStream.Send("re-routed");
				httpStream.Close();
			}
		}

		public static string DataPath => Directory.GetCurrentDirectory() + "/Data/";

		public static readonly Dictionary<string, OperationBase> Operations = new Dictionary<string, OperationBase>()
		{
			{ "test", new TestOperation() },
			{ "img", new ImageOperation() }
		};
		

	}
}
