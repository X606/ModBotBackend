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
using PlayFab;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace ModBotBackend
{
	static class Program
	{
		static void Main(string[] args)
		{
			PopulateOperations();

			Directory.CreateDirectory(BasePath);

			Directory.CreateDirectory(UsersPath);
			Directory.CreateDirectory(DataPath);

			UploadedModsManager.Setup(DataPath);
			UserManager.Init();

			TemporaryFilesMananger.Init();

			HttpListener httpsListener = new HttpListener();
			//httpListener.Prefixes.Add("http://+:80/");
			httpsListener.Prefixes.Add("https://+:443/");
			httpsListener.Start();
			listenHttps(httpsListener);

			OutputConsole.WriteLine("Listening...");

			HttpListener httpListener = new HttpListener();
			httpListener.Prefixes.Add("http://+:80/");
			httpListener.Start();
			listenHttp(httpListener);

			while (true)
				System.Threading.Thread.Sleep(1000 * 60 * 60);
		}

		static async void listenHttps(HttpListener httpListener)
		{
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

			Authentication authentication;
			if (SessionsManager.VerifyKey(sessionID, out Session session))
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
						selectedOperation.OnOperation(context, authentication);
						
					}
					catch(Exception e)
					{
						try
						{
							string error = e.ToString();

							error = error.Replace("\"", "\\\"");


							HttpStream httpStream = new HttpStream(context.Response);

							//OutputConsole.WriteLine(e.ToString());

							httpStream.Send("{\"isError\":true}");
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

					Utils.RederectToErrorPage(context, "invalid operation \"" + operation + "\"");
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

		public static string DataPath => BasePath + "/Data/";
		public static string UsersPath => BasePath + "/Users/";
		public static string DiscordClientSecretPath => BasePath + "/discordSecret.txt";

		public static string ModTemplateFilePath => BasePath + "/ModTemplate/";
		public static string TemporaryFiles => BasePath + "/TemporaryFiles/";

		public static string WebsitePath => BasePath + "/Website/";
		public static string WebsiteFile => BasePath + "/Website.txt";

		public static void PopulateOperations()
		{
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

		public static readonly Dictionary<string, OperationBase> Operations = new Dictionary<string, OperationBase>();
		

	}
}
