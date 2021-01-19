using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net;
using System.IO;
using ModBotBackend.Operations;
using ModBotBackend.Users;
using ModBotBackend.Users.Sessions;
using System.Diagnostics;
using System.Net.Sockets;
using Newtonsoft.Json;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace ModBotBackend
{
	static class Program
	{
		static void Main(string[] args)
		{
			PopulateOperations();
			PopulateOwnFolderObjects();

			Directory.CreateDirectory(BasePath);

			Directory.CreateDirectory(ModTemplateFilePath);
			Directory.CreateDirectory(WebsitePath);

			if (args.Contains("-httpOnly"))
			{
				HttpListener httpMainListener = new HttpListener();
				httpMainListener.Prefixes.Add("http://+:80/");
				httpMainListener.Start();
				listenMain(httpMainListener);

				OutputConsole.WriteLine("Listening...");
				OutputConsole.WriteLine("WARNING: You are currently running in http only mode, this is only intended for test use, do not do this in production.");

				while (true)
					System.Threading.Thread.Sleep(1000 * 60 * 60);

			}


			HttpListener httpsListener = new HttpListener();
			//httpListener.Prefixes.Add("http://+:80/");
			httpsListener.Prefixes.Add("https://+:443/");
			httpsListener.Start();
			listenMain(httpsListener);

			OutputConsole.WriteLine("Listening...");

			HttpListener httpListener = new HttpListener();
			httpListener.Prefixes.Add("http://+:80/");
			httpListener.Start();
			listenHttp(httpListener);

			while (true)
				System.Threading.Thread.Sleep(1000 * 60 * 60);
		}

		static async void listenMain(HttpListener httpListener)
		{
			Task.Factory.StartNew(() => DeployerComunication());
			while(true)
			{
				var context = await httpListener.GetContextAsync();
				//Console.WriteLine("Client connected");
				Task.Factory.StartNew(() => processRequest(context));
			}
		}
		static async void listenHttp(HttpListener httpListener)
		{
			while (true)
			{
				var context = await httpListener.GetContextAsync();
				//Console.WriteLine("Client connected");
				Task.Factory.StartNew(() =>
				{
					UriBuilder builder = new UriBuilder(context.Request.Url);

					builder.Scheme = "https";
					builder.Port = 443;

					string url = builder.Uri.ToString();

					context.Response.Redirect(url);
					HttpStream httpStream = new HttpStream(context.Response);
					httpStream.Send("Re-rounted to https");
					httpStream.Close();
				});
			}
		}

		static string GetCookie(HttpListenerRequest request, string cookieName)
		{
			for (int i = 0; i < request.Cookies.Count; i++)
			{
				if (request.Cookies[i].Name == cookieName)
					return request.Cookies[i].Value;
			}

			return null;
		}

		static void processRequest(HttpListenerContext context)
		{
			HttpListenerRequest request = context.Request;

			string sessionID = GetCookie(request,"SessionID");

			//OutputConsole.WriteLine("sessionID: " + sessionID);

			Authentication authentication;
			if (SessionsManager.Instance.VerifyKey(sessionID, out Session session))
			{
				authentication = new Authentication(session.AuthenticationLevel, session.OwnerUserID, sessionID);
			} else
			{
				authentication = new Authentication(AuthenticationLevel.None, "", "");
			}

			string operation = request.QueryString.Get("operation");
			
			string absolutePath = request.Url.AbsolutePath;
			if(absolutePath == "/api/" || absolutePath == "/api")
			{
				if(operation != null && Operations.TryGetValue(operation, out OperationBase selectedOperation))
				{
					try
					{
						Stopwatch stopwatch = new Stopwatch();
						stopwatch.Start();
						if (selectedOperation.ParseAsJson)
						{
							context.Response.ContentType = "application/json";
						}

						selectedOperation.OnOperation(context, authentication);
						stopwatch.Stop();
					}
					catch(Exception e)
					{
						try
						{
							string error = e.ToString();

							error = error.Replace("\"", "\\\"");

							context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
							HttpStream httpStream = new HttpStream(context.Response);

							//OutputConsole.WriteLine(e.ToString());

							string errorJson = new InternalError(e.ToString()).ToJson();

							httpStream.Send(errorJson);
							httpStream.Close();

						}
						catch
						{
							context.Response.Abort();
							// At this point just forget it and move on
							return;
						}
						//Utils.RederectToErrorPage(context, "an error occured");
					}
				}
				else
				{
					if(operation == null)
						operation = "null";

					Utils.SendErrorPage(context.Response, "invalid operation \"" + operation + "\"", true, HttpStatusCode.BadRequest);
				}
			} else
			{
				try
				{
					WebsiteRequestProcessor.OnRequest(context);
				} catch(Exception e)
				{
#if DEBUG
					//OutputConsole.WriteLine("\n" + e.ToString());
#endif
				}
				
				
			}
		}

		public static string BasePath => Directory.GetCurrentDirectory() + "/SiteData/";

		public static string ModTemplateFilePath => BasePath + "/ModTemplate/";
		public static string WebsitePath => BasePath + "/Website/";
		public static string WebsiteFile => BasePath + "/Website.txt";


		public static void PopulateOperations()
		{
			Operations.Clear();

			Type[] loadedTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
			for (int i = 0; i < loadedTypes.Length; i++)
			{
				OperationAttribute operationAttribute = (OperationAttribute)Attribute.GetCustomAttribute(loadedTypes[i], typeof(OperationAttribute));
				if (operationAttribute == null)
					continue;

				if (loadedTypes[i].BaseType != typeof(OperationBase))
					continue;

				Operations.Add(operationAttribute.OperationKey, (OperationBase)Activator.CreateInstance(loadedTypes[i]));
			}

		}
		public static void PopulateOwnFolderObjects()
		{
			OwnFolderObjects.Clear();

			Type[] loadedTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
			for (int i = 0; i < loadedTypes.Length; i++)
			{
				FolderNameAttribute operationAttribute = (FolderNameAttribute)Attribute.GetCustomAttribute(loadedTypes[i], typeof(FolderNameAttribute));
				if (operationAttribute == null)
					continue;

				object instance = Activator.CreateInstance(loadedTypes[i]);
				instance.GetType().GetMethod("Init").Invoke(instance, new object[] { operationAttribute.FolderName });
				OwnFolderObjects.Add(instance);
			}

		}

		static void OnProcessExit()
		{
			for (int i = 0; i < OwnFolderObjects.Count; i++)
			{
				OwnFolderObjects[i].GetType().GetMethod("OnShutDown").Invoke(OwnFolderObjects[i], new object[] { });
			}

		}
		static void DeployerComunication()
		{
			IPHostEntry host = Dns.GetHostEntry("localhost");
			IPAddress ipAddress = host.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, 12022);

			// Create a TCP/IP  socket.    
			Socket sender = new Socket(ipAddress.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

			// Connect to Remote EndPoint  
			sender.Connect(remoteEP);

			byte[] buffer = new byte[4];
			sender.Receive(buffer);
			Console.WriteLine("Exiting....");

			OnProcessExit();
			Environment.Exit(0);

		}

		public static readonly Dictionary<string, OperationBase> Operations = new Dictionary<string, OperationBase>();
		public static readonly List<object> OwnFolderObjects = new List<object>();

	}

	[Serializable]
	public class InternalError
	{
		public InternalError(string _error)
		{
			error = _error;
		}
		public string ToJson()
		{
			return JsonConvert.SerializeObject(this);
		}
		public bool isError = true;
		public string error;
	}
}
