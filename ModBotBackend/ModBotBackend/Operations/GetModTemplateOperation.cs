﻿using System;
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
	public class GetModTemplateOperation : OperationBase
	{

		public override void OnOperation(HttpListenerContext context)
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

			if(!SessionsManager.VerifyKey(request.sessionId, out Session session))
			{
				HttpStream http = new HttpStream(context.Response);
				http.Send(new GetModTemplateRequestResponse()
				{
					isError = false,
					message = "The provided session id was either outdated or invalid"
				}.ToJson());
				http.Close();
				return;
			}

			string creatorUsername = UserManager.GetUserFromId(session.OwnerUserID).Username;

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

			TemporaryFilesMananger.CreateTemporaryFile(zipFilePath, out string key);

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
			public string sessionId;
			public string modName;
			public string description;
			public string[] tags;

			public bool IsValidRequest()
			{
				return !string.IsNullOrWhiteSpace(sessionId) && !string.IsNullOrWhiteSpace(modName) && !string.IsNullOrWhiteSpace(description);
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
