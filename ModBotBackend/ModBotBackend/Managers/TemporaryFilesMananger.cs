using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ModBotBackend
{
    [FolderName("TemporaryFiles")]
    public class TemporaryFilesMananger : OwnFolderObject<TemporaryFilesMananger>
    {
        public override void OnStartup()
        {
            string path = FolderPath;
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        Dictionary<string, TempFile> _tempFiles = new Dictionary<string, TempFile>();

        public bool CreateTemporaryFile(string path, out string key)
        {
            string[] subPaths = path.Split('/', '\\');
            string fileName = subPaths[subPaths.Length - 1];

            string generatedKey = Utils.GenerateSecureKey();

            byte[] data;
            try
            {
                data = File.ReadAllBytes(path);
            }
            catch
            {
                key = "";
                return false;
            }

            string filename = generatedKey;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c.ToString(), "");
            }
            string tempFilePath = FolderPath + filename;

            File.WriteAllBytes(tempFilePath, data);

            _tempFiles.Add(generatedKey, new TempFile()
            {
                fileName = fileName,
                path = tempFilePath
            });

            Task.Factory.StartNew(() => deleteFileAfterSeconds(generatedKey, 30));

            key = generatedKey;

            return true;
        }

        async void deleteFileAfterSeconds(string key, float seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));

            DeleteTempFile(key);
        }

        public bool TryGetTempFile(string key, out byte[] data, out string filename)
        {
            if (key == null || !_tempFiles.ContainsKey(key))
            {
                data = null;
                filename = null;
                return false;
            }

            TempFile tempFile = _tempFiles[key];

            data = File.ReadAllBytes(tempFile.path);
            filename = tempFile.fileName;

            DeleteTempFile(key);

            return true;
        }

        public void DeleteTempFile(string key)
        {
            if (_tempFiles.ContainsKey(key))
            {
                File.Delete(_tempFiles[key].path);
                _tempFiles.Remove(key);
            }
        }
    }

    public class TempFile
    {
        public string fileName;
        public string path;
    }



}
