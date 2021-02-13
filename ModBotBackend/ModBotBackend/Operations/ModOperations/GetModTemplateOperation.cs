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
	public class GetModTemplateOperation : JsonOperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { "modName", "description", "tags" };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.BasicUser;
		public override string OverrideResolveJavascript =>
			"if(!e.isError){" +
				"API.downloadTempFile(e.fileKey);" +
				"resolve({ isError: false, message: \"Downloading file...\" });" +
				"return;" +
			"} " +
			"resolve(e);";

		public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
		{
			GetModTemplateRequestData request = new GetModTemplateRequestData()
			{
				description = arguments["description"],
				modName = arguments["modName"],
				tags = arguments["tags"]
			};

			if (!request.IsValidRequest())
			{
				return new GetModTemplateRequestResponse()
				{
					Error = "The request was invalid"
				};
			}

			if(!authentication.HasAtLeastAuthenticationLevel(AuthenticationLevel.BasicUser))
			{
				return new GetModTemplateRequestResponse()
				{
					Error = "You are not signed in"
				};
			}

			string creatorUsername = UserManager.Instance.GetUserFromId(authentication.UserID).Username;

			ModInfoCopy modInfo = new ModInfoCopy()
			{
				DisplayName = request.modName,
				UniqueID = Guid.NewGuid().ToString(),
				MainDLLFileName = "",
				Author = creatorUsername,
				Description = request.description,
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

			File.Delete(zipFilePath);
			Utils.RecursivelyDeleteFolder(tempPath);

			return new GetModTemplateRequestResponse()
			{
				fileKey = key
			};
		}

		[Serializable]
		private class GetModTemplateRequestData
		{
			public string modName;
			public string description;
			public string[] tags;

			public bool IsValidRequest()
			{
				if (description == null)
					description = "";
				if (tags == null)
					tags = new string[0];

				return !string.IsNullOrWhiteSpace(modName) && !modName.Contains("/") && !modName.Contains("\\");
			}
		}
		[Serializable]
		private class GetModTemplateRequestResponse : JsonOperationResponseBase
		{
			public string message;

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
