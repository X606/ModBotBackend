using System;
using System.IO;

namespace ModBotBackend.Managers
{

    [FolderName("ModBotInfo")]
    public class ModBotInfoManager : OwnFolderObject<ModBotInfoManager>
    {
        const string DATA_FILENAME = "ModBotInfo.dat";

        string _dataFilePath => FolderPath + DATA_FILENAME;

        ModBotInfoSaveData _saveData = new ModBotInfoSaveData();

        public override void OnStartup()
        {
            if (File.Exists(_dataFilePath))
            {
                BitCompressor.FillObj(_saveData, File.ReadAllBytes(_dataFilePath));
            }
        }
        void Save()
        {
            byte[] bytes = BitCompressor.GetBytes(_saveData);
            File.WriteAllBytes(_dataFilePath, bytes);
        }

        public void SetModBotVersion(string modBotVersion)
        {
            _saveData.ModBotVersion = modBotVersion;
            Save();
        }
        public string GetModBotVersion()
        {
            return _saveData.ModBotVersion;
        }

        public void SetModBotDownloadLink(string downloadLink)
        {
            _saveData.ModBotDownloadLink = downloadLink;
            Save();
        }
        public string GetModBotDownloadLink()
        {
            return _saveData.ModBotDownloadLink;
        }

        public void SetModBotLauncherDownloadLink(string downloadLink)
        {
            _saveData.ModBotLauncherDownloadLink = downloadLink;
            Save();
        }
        public string GetModBotLauncherDownloadLink()
        {
            return _saveData.ModBotLauncherDownloadLink;
        }


    }

    [Serializable]
    public class ModBotInfoSaveData
    {
        public string ModBotVersion = null;
        public string ModBotDownloadLink = null;
        public string ModBotLauncherDownloadLink = null;
    }
}
