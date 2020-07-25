using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend.JsonDataTypes;
using System.IO;

namespace ModBotBackend
{
	public static class UploadedModsManager
	{
		public const string MOD_DATA_FILE_NAME = "ModData.json";

		static string _dataPath;

		public static void Setup(string dataPath)
		{
			_dataPath = dataPath;
		}

		public static void CreateMod(Mod modData, ref string error)
		{
			string modsFolderPath = createModsFolder();

			string uuid = modData.UniqueID;
			string newModFolderPath = Utils.CombinePaths(modsFolderPath, uuid);
			if (Directory.Exists(newModFolderPath))
			{
				error = "Mod with the uuid \"" + uuid + "\" already exists";
				return;
			}

			string jsonPath = Utils.CombinePaths(newModFolderPath, MOD_DATA_FILE_NAME);
			File.WriteAllText(jsonPath, modData.Serilize());

		}


		static string createModsFolder()
		{
			string path = Utils.CombinePaths(_dataPath, "Mods/");
			Directory.CreateDirectory(path);

			return path;
		}

	}
}
