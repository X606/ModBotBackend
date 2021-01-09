using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HttpUtils;
using ModLibrary;
using ModBotBackend.Users;
using ModBotBackend.Users.Sessions;
using Newtonsoft.Json;
using System.IO.Compression;

namespace ModBotBackend.Operations
{
	[Operation("getModTemplate")]
	public class GetModTemplateOperation : OperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "modName", "description", "tags" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
		public override string OverrideResolveJavascript => 
			"if(e.isError){" +
			"resolve(e);" +
			"return;"+
			"} " +
			"API.downloadTempFile(e.fileKey);"+
			"resolve({ isError: false, message: \"Downloading file...\" });";

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			GetModTemplateRequestData request = JsonConvert.DeserializeObject<GetModTemplateRequestData>(json);

			if(!request.IsValidRequest())
			{
				HttpStream http = new HttpStream(context.Response);
				http.Send(new GetModTemplateRequestResponse()
				{
					isError = false,
					message = "The request was invalid"
				}.ToJson());
				http.Close();
				return;
			}

			if(!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.BasicUser))
			{
				HttpStream http = new HttpStream(context.Response);
				http.Send(new GetModTemplateRequestResponse()
				{
					isError = false,
					message = "You are not signed in"
				}.ToJson());
				http.Close();
				return;
			}

			string creatorUsername = UserManager.Instance.GetUserFromId(authentication.UserID).Username;

			ModInfoCopy modInfo = new ModInfoCopy()
			{
				DisplayName = request.modName,
				UniqueID = Guid.NewGuid().ToString(),
				MainDLLFileName = "",
				Author = creatorUsername,
				Description = "",
				ImageFileName = "DefaultImage.png",
				ModDependencies = new string[0],
				Tags = request.tags,
				Version = 0
			};


			string modInfoJson = JsonConvert.SerializeObject(modInfo, Formatting.Indented);

			string tempPath = Path.GetTempPath() + modInfo.UniqueID + "/";
			Directory.CreateDirectory(tempPath);

			Utils.CopyFilesRecursively(new DirectoryInfo(Program.ModTemplateFilePath), new DirectoryInfo(tempPath));
			
			File.WriteAllText(tempPath + InternalModBot.ModsManager.MOD_INFO_FILE_NAME, modInfoJson);

			string zipFileName = modInfo.DisplayName;
			foreach(char c in Path.GetInvalidFileNameChars())
			{
				zipFileName = zipFileName.Replace(c.ToString(), "");
			}
			string zipFilePath = Path.GetTempPath() + zipFileName + ".zip";

			if (File.Exists(zipFilePath))
			{
				File.Delete(zipFilePath);
			}
			
			ZipFile.CreateFromDirectory(tempPath, zipFilePath);

			TemporaryFilesMananger.Instance.CreateTemporaryFile(zipFilePath, out string key);

			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(new GetModTemplateRequestResponse()
			{
				isError = false,
				fileKey = key
			}.ToJson());
			httpStream.Close();

			File.Delete(zipFilePath);
			Utils.RecursivelyDeleteFolder(tempPath);
		}

		[Serializable]
		private class GetModTemplateRequestData
		{
			public string modName;
			public string description;
			public string[] tags;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(modName) && !string.IsNullOrWhiteSpace(description);
			}
		}
		[Serializable]
		private class GetModTemplateRequestResponse
		{
			public string message;
			public bool isError;

			public string fileKey;

			public string ToJson()
			{
				return JsonConvert.SerializeObject(this);
			}
		}

		private class ModInfoCopy
		{
			[JsonProperty]
			[JsonRequired]
			public string DisplayName;
			[JsonProperty]
			[JsonRequired]
			public string UniqueID;
			[JsonProperty]
			[JsonRequired]
			public string MainDLLFileName;
			[JsonProperty]
			[JsonRequired]
			public string Author;
			[JsonProperty]
			[JsonRequired]
			public uint Version;
			[JsonProperty]
			public string ImageFileName;
			[JsonProperty]
			public string Description;
			[JsonProperty]
			public string[] ModDependencies;
			[JsonProperty]
			public string[] Tags;
		}

	}
}
