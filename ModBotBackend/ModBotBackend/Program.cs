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

namespace ModBotBackend
{
	static class Program
	{
		static void Main(string[] args)
		{
			Directory.CreateDirectory(UsersPath);
			Directory.CreateDirectory(DataPath);

			UploadedModsManager.Setup(DataPath);
			UserManager.Init();

			TemporaryFilesMananger.Init();

			HttpListener httpListener = new HttpListener();
			httpListener.Prefixes.Add("http://+:80/");
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
					catch(Exception e)
					{
						try
						{
							string error = e.ToString();

							error = error.Replace("\"", "\\\"");


							HttpStream httpStream = new HttpStream(context.Response);

							Console.WriteLine(e.ToString());

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
					Console.WriteLine("\n" + e.ToString());
#endif
				}
				
				
			}
		}

		public static string DataPath => Directory.GetCurrentDirectory() + "/Data/";
		public static string UsersPath => Directory.GetCurrentDirectory() + "/Users/";
		public static string DiscordClientSecretPath => Directory.GetCurrentDirectory() + "/discordSecret.txt";

		public static string ModTemplateFilePath => Directory.GetCurrentDirectory() + "/ModTemplate/";
		public static string TemporaryFiles => Directory.GetCurrentDirectory() + "/TemporaryFiles/";

		public static readonly Dictionary<string, OperationBase> Operations = new Dictionary<string, OperationBase>()
		{
			{ "test", new TestOperation() },
			{ "uploadMod", new UploadModOperation() },
			{ "getModData", new GetModDataOperation() },
			{ "getModImage", new GetImageOperation() },
			{ "downloadMod", new DownloadModOperation() },
			{ "getAllModIds", new GetAllModIdsOperation() },
			{ "getAllModInfos", new GetAllModInfosOperation() },
			{ "createAccout", new CreateAccountOperation() },
			{ "signIn", new SignInOperation() },
			{ "getSpecialModData", new GetSpecialModDataOperation() },
			{ "like", new LikeOperation() },
			{ "hasLiked", new HasLikedOperation() },
			{ "isValidSession", new IsValidSessionOperation() },
			{ "signOut", new SignOutOperation() },
			{ "postComment", new PostCommentOperation() },
			{ "deleteComment", new DeleteCommentOperation() },
			{ "likeComment", new LikeCommentOperation() },
			{ "getUser", new GetPublicUserDataOperation() },
			{ "getProfilePicture", new GetProfilePictureOperation() },
			{ "hasLikedComment", new HasLikedCommentOperation() },
			{ "isCommentMine", new IsMyCommentOperation() },
			{ "getCurrentUser", new GetCurrentUserOperation() },
			{ "getModTemplate", new GetModTemplateOperation() },
			{ "downloadTempFile", new DownloadTempFileOperation() },
			{ "search", new ModSearchOperation() },
			{ "uploadProfilePicture", new UploadProfilePictureOperation() },
			{ "updateUserData", new UpdateUserDataOperation() },
			{ "favoriteMod", new FavoriteModOperation() }
		};
		

	}
}
