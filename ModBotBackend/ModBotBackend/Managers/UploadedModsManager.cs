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
	[FolderName("Data")]
	public class UploadedModsManager : OwnFolderObject<UploadedModsManager>
	{
		static string _dataPath;

		static Dictionary<string, ModInfo> _loadedMods = new Dictionary<string, ModInfo>();
		static Dictionary<string, string> _loadedModJsons = new Dictionary<string, string>();

		static Dictionary<string, SpecialModData> _loadedSpecialModData = new Dictionary<string, SpecialModData>();

		public override void OnStartup()
		{
			string dataPath = FolderPath;

			_dataPath = dataPath;

			GetOrCreateZippedModsFolder();
			GetOrCreateModsFolder();
			GetOrCreateSpecialModsDataFolder();

			string modsFolder = GetOrCreateModsFolder();
			string[] modFolders = Directory.GetDirectories(modsFolder);
			foreach (string folder in modFolders)
			{
				string dataFilePath = folder + "/" + ModsManager.MOD_INFO_FILE_NAME;
				if (!File.Exists(dataFilePath))
					continue;

				ModInfo loadedModInfo;
				string json = File.ReadAllText(dataFilePath);
				try
				{
					loadedModInfo = JsonConvert.DeserializeObject<ModInfo>(json);
				}
				catch (JsonException e)
				{
					OutputConsole.WriteLine("Failed to read mod data file for mod: " + folder);
					continue;
				}
				if (!loadedModInfo.AreAllEssentialFieldsAssigned(out string msg))
				{
					OutputConsole.WriteLine("Missing fields for mod: " + folder + ", " + msg);
					continue;
				}
				loadedModInfo.FixFieldValues();

				_loadedMods.Add(loadedModInfo.UniqueID, loadedModInfo);
				_loadedModJsons.Add(loadedModInfo.UniqueID, json);
			}

			string specialModDataFolder = GetOrCreateSpecialModsDataFolder();
			string[] specialModDataFiles = Directory.GetFiles(specialModDataFolder);
			foreach (string specialModDatFile in specialModDataFiles)
			{
				string json = File.ReadAllText(specialModDatFile);
				SpecialModData specialModData = SpecialModData.FromJson(json);
				_loadedSpecialModData.Add(specialModData.ModId, specialModData);
			}

			foreach (KeyValuePair<string, ModInfo> loadedMod in _loadedMods)
			{
				ZipMod(loadedMod.Key);
			}
		}
		
		public KeyValuePair<SpecialModData, ModInfo>[] GetAllUploadedMods()
		{
			if(_loadedSpecialModData.Count != _loadedMods.Count)
				throw new Exception("the length of special mod datas is diffrent than the length of normal mod datas");

			SpecialModData[] a = _loadedSpecialModData.Values.ToArray();
			ModInfo[] b = _loadedMods.Values.ToArray();

			List<KeyValuePair<SpecialModData, ModInfo>> mods = new List<KeyValuePair<SpecialModData, ModInfo>>();
			for(int i = 0; i < a.Length; i++)
			{
				mods.Add(new KeyValuePair<SpecialModData, ModInfo>(a[i], b[i]));
			}

			return mods.ToArray();
		}

		public bool TryUploadModFromZip(byte[] zipData, string sessionID, out ModInfo modInfo, out string error)
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


				string[] directories = Directory.GetDirectories(tempModPath);
				if(Directory.GetFiles(tempModPath).Length == 0 && directories.Length == 1) // if there is a single folder and no files then the mod is probably in that folder
				{
					tempModPath = directories[0];
				}

				string modInfoPath = tempModPath + ModsManager.MOD_INFO_FILE_NAME;

				if (!File.Exists(modInfoPath))
				{
					error = "Could not find the " + ModsManager.MOD_INFO_FILE_NAME + " file :/";
					modInfo = null;
					return false;
				}

				string json = File.ReadAllText(modInfoPath);
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

				string playerId = SessionsManager.GetPlayerIdFromSession(sessionID);
				
				SpecialModData oldModWithSameID = GetSpecialModInfoFromId(loadedModInfo.UniqueID);
				if (oldModWithSameID != null)
				{
					if (oldModWithSameID.OwnerID != playerId)
					{
						error = "You cannot override a mod that you do not own";
						modInfo = null;
						return false;
					}
				}

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
				error = "something went wrong while handaling the zip file";
				OutputConsole.WriteLine(e.ToString());
				return false;
			}

			if (_loadedMods.ContainsKey(loadedModInfo.UniqueID))
				_loadedMods.Remove(loadedModInfo.UniqueID);
			if(_loadedModJsons.ContainsKey(loadedModInfo.UniqueID))
				_loadedModJsons.Remove(loadedModInfo.UniqueID);
			
			_loadedMods.Add(loadedModInfo.UniqueID, loadedModInfo);
			_loadedModJsons.Add(loadedModInfo.UniqueID, JsonConvert.SerializeObject(loadedModInfo));

			ZipMod(loadedModInfo.UniqueID);
			
			if(_loadedSpecialModData.ContainsKey(loadedModInfo.UniqueID))
			{
				_loadedSpecialModData[loadedModInfo.UniqueID].UpdatedDate = (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			} else
			{
				CreateAndAddSpecialModDataFormMod(loadedModInfo.UniqueID, SessionsManager.GetPlayerIdFromSession(sessionID));
			}

			modInfo = loadedModInfo;
			error = null;
			return true;
		}
		
		public string GetModInfoJsonFromId(string id)
		{
			if(!_loadedModJsons.ContainsKey(id))
				return "null";
			
			return _loadedModJsons[id];
		}
		public ModInfo GetModInfoFromId(string id)
		{
			if(!_loadedMods.ContainsKey(id))
				return null;

			return _loadedMods[id];
		}

		public SpecialModData GetSpecialModInfoFromId(string id)
		{
			if(!_loadedSpecialModData.ContainsKey(id))
				return null;

			return _loadedSpecialModData[id];
		}
		public string GetSpecialModInfoJsonFromId(string id)
		{
			SpecialModData modData = GetSpecialModInfoFromId(id);

			if(modData == null)
				return null;

			return modData.ToJson();
		}

		public string GetModPathFromID(string id)
		{
			return GetOrCreateModsFolder() + id + "/";
		}
		public string GetZippedModPathFromID(string id)
		{
			return GetOrCreateZippedModsFolder() + id + ".zip";
		}

		public string[] GetAllUploadedIds()
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

		public bool HasModWithIdBeenUploaded(string id)
		{
			return _loadedMods.ContainsKey(id);
		}

		public void ZipMod(string id)
		{
			if(!HasModWithIdBeenUploaded(id))
				return;

			ModInfo modInfo = GetModInfoFromId(id);

			string zippedModPath = GetOrCreateZippedModsFolder() + id + ".zip";

			if(File.Exists(zippedModPath))
				File.Delete(zippedModPath);

			ZipFile.CreateFromDirectory(GetOrCreateModsFolder() + id, zippedModPath);
		}
		public void CreateAndAddSpecialModDataFormMod(string modID, string ownerID)
		{
			ModInfo modInfo = GetModInfoFromId(modID);
			SpecialModData specialModData = SpecialModData.CreateNewSpecialModData(modInfo, ownerID);

			_loadedSpecialModData.Add(specialModData.ModId, specialModData);

			SaveSpecialModData(specialModData);
		}
		public void SaveSpecialModData(SpecialModData data)
		{
			string path = GetOrCreateSpecialModsDataFolder() + data.ModId + ".json";

			File.WriteAllText(path, data.ToJson());
		}

		public string GetOrCreateModsFolder()
		{
			string path = Utils.CombinePaths(_dataPath, "Mods/");
			Directory.CreateDirectory(path);

			return path;
		}
		public string GetOrCreateZippedModsFolder()
		{
			string path = Utils.CombinePaths(_dataPath, "ZippedMods/");
			Directory.CreateDirectory(path);

			return path;
		}
		public string GetOrCreateSpecialModsDataFolder()
		{
			string path = Utils.CombinePaths(_dataPath, "SpecialModsData/");
			Directory.CreateDirectory(path);

			return path;
		}

	}
}
