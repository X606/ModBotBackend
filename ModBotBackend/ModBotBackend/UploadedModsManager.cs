using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using InternalModBot;
using ModLibrary;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Collections;
using ModBotBackend.Users.Sessions;

namespace ModBotBackend
{
	public static class UploadedModsManager
	{
		static string _dataPath;

		static Dictionary<string, ModInfo> _loadedMods = new Dictionary<string, ModInfo>();
		static Dictionary<string, string> _loadedModJsons = new Dictionary<string, string>();

		static List<SpecialModData> _loadedSpecialModData = new List<SpecialModData>();

		public static void Setup(string dataPath)
		{
			_dataPath = dataPath;

			GetOrCreateZippedModsFolder();
			GetOrCreateModsFolder();
			GetOrCreateSpecialModsDataFolder();

			string modsFolder = GetOrCreateModsFolder();
			string[] modFolders = Directory.GetDirectories(modsFolder);
			foreach(string folder in modFolders)
			{
				string dataFilePath = folder +"/" + ModsManager.MOD_INFO_FILE_NAME;
				if(!File.Exists(dataFilePath))
					continue;

				ModInfo loadedModInfo;
				string json = File.ReadAllText(dataFilePath);
				try
				{
					loadedModInfo = JsonConvert.DeserializeObject<ModInfo>(json);
				}
				catch(JsonException e)
				{
					Console.WriteLine("Failed to read mod data file for mod: " + folder);
					continue;
				}
				if(!loadedModInfo.AreAllEssentialFieldsAssigned(out string msg))
				{
					Console.WriteLine("Missing fields for mod: " + folder + ", " + msg);
					continue;
				}
				loadedModInfo.FixFieldValues();

				_loadedMods.Add(loadedModInfo.UniqueID, loadedModInfo);
				_loadedModJsons.Add(loadedModInfo.UniqueID, json);
			}

			string specialModDataFolder = GetOrCreateSpecialModsDataFolder();
			string[] specialModDataFiles = Directory.GetFiles(specialModDataFolder);
			foreach(string specialModDatFile in specialModDataFiles)
			{
				string json = File.ReadAllText(specialModDatFile);
				SpecialModData specialModData = SpecialModData.FromJson(json);
				_loadedSpecialModData.Add(specialModData);
			}

			foreach(KeyValuePair<string, ModInfo> loadedMod in _loadedMods)
			{
				ZipMod(loadedMod.Key);
			}
			
		}
		
		public static bool TryUploadModFromZip(byte[] zipData, string sessionID, out ModInfo modInfo, out string error)
		{
			ModInfo loadedModInfo;
			try
			{
				string tempPath = Path.GetTempPath();
				string guid = Guid.NewGuid().ToString();

				string tempZipPath = tempPath + guid + ".zip";
				string tempModPath = tempPath + guid + "/";
				Directory.CreateDirectory(tempModPath);

				File.WriteAllBytes(tempZipPath, zipData);
				ZipFile.ExtractToDirectory(tempZipPath, tempModPath);

				string json = File.ReadAllText(tempModPath + ModsManager.MOD_INFO_FILE_NAME);
				try
				{
					loadedModInfo = JsonConvert.DeserializeObject<ModInfo>(json);
				}
				catch(JsonException e)
				{
					error = "Error deserializing" + ModsManager.MOD_INFO_FILE_NAME + " file";
					modInfo = null;
					return false;
				}
				if(!loadedModInfo.AreAllEssentialFieldsAssigned(out string msg))
				{
					error = msg;
					modInfo = null;
					return false;
				}
				loadedModInfo.FixFieldValues();

				string newFolderPath = GetOrCreateModsFolder() + loadedModInfo.UniqueID + "/";
				if(Directory.Exists(newFolderPath))
					Utils.RecursivelyDeleteFolder(newFolderPath);

				Directory.CreateDirectory(newFolderPath);

				Utils.CopyFilesRecursively(new DirectoryInfo(tempModPath), new DirectoryInfo(newFolderPath));

				File.Delete(tempZipPath);
				Utils.RecursivelyDeleteFolder(tempModPath);
			}
			catch(Exception e)
			{
				modInfo = null;
				error = "something went wrong while handeling the zip file";
				Console.WriteLine(e.Message);
				return false;
			}

			if (_loadedMods.ContainsKey(loadedModInfo.UniqueID))
				_loadedMods.Remove(loadedModInfo.UniqueID);

			_loadedMods.Add(loadedModInfo.UniqueID, loadedModInfo);

			ZipMod(loadedModInfo.UniqueID);
			CreateAndAddSpecialModDataFormMod(loadedModInfo.UniqueID, SessionsManager.GetPlayerIdFromSession(sessionID));

			modInfo = loadedModInfo;
			error = null;
			return true;
		}
		
		public static string GetModInfoJsonFromId(string id)
		{
			if(!_loadedModJsons.ContainsKey(id))
				return "null";
			
			return _loadedModJsons[id];
		}
		public static ModInfo GetModInfoFromId(string id)
		{
			if(!_loadedMods.ContainsKey(id))
				return null;

			return _loadedMods[id];
		}

		public static SpecialModData GetSpecialModInfoFromId(string id)
		{
			foreach(SpecialModData modData in _loadedSpecialModData)
			{
				if(modData.ModId == id)
					return modData;
			}

			return null;
		}
		public static string GetSpecialModInfoJsonFromId(string id)
		{
			SpecialModData modData = GetSpecialModInfoFromId(id);

			if(modData == null)
				return null;

			return modData.ToJson();
		}

		public static string GetModPathFromID(string id)
		{
			return GetOrCreateModsFolder() + id + "/";
		}
		public static string GetZippedModPathFromID(string id)
		{
			return GetOrCreateZippedModsFolder() + id + ".zip";
		}

		public static string[] GetAllUploadedIds()
		{
			string[] ids = new string[_loadedMods.Count];

			int i = 0;
			foreach(KeyValuePair<string, ModInfo> item in _loadedMods)
			{
				ids[i] = item.Key;
				i++;
			}

			return ids;
		}

		public static bool HasModWithIdBeenUploaded(string id)
		{
			return _loadedMods.ContainsKey(id);
		}

		public static void ZipMod(string id)
		{
			if(!HasModWithIdBeenUploaded(id))
				return;

			ModInfo modInfo = GetModInfoFromId(id);

			string zippedModPath = GetOrCreateZippedModsFolder() + id + ".zip";

			if(File.Exists(zippedModPath))
				File.Delete(zippedModPath);

			ZipFile.CreateFromDirectory(GetOrCreateModsFolder() + id, zippedModPath);
		}
		public static void CreateAndAddSpecialModDataFormMod(string modID, string ownerID)
		{
			ModInfo modInfo = GetModInfoFromId(modID);
			SpecialModData specialModData = SpecialModData.CreateNewSpecialModData(modInfo, ownerID);

			_loadedSpecialModData.Add(specialModData);

			SaveSpecialModData(specialModData);
		}
		public static void SaveSpecialModData(SpecialModData data)
		{
			string path = GetOrCreateSpecialModsDataFolder() + data.ModId + ".json";

			File.WriteAllText(path, data.ToJson());
		}

		public static string GetOrCreateModsFolder()
		{
			string path = Utils.CombinePaths(_dataPath, "Mods/");
			Directory.CreateDirectory(path);

			return path;
		}
		public static string GetOrCreateZippedModsFolder()
		{
			string path = Utils.CombinePaths(_dataPath, "ZippedMods/");
			Directory.CreateDirectory(path);

			return path;
		}
		public static string GetOrCreateSpecialModsDataFolder()
		{
			string path = Utils.CombinePaths(_dataPath, "SpecialModsData/");
			Directory.CreateDirectory(path);

			return path;
		}

	}
}
