using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Managers
{
    [FolderName("notes")]
    public class NotesManager : OwnFolderObject<NotesManager>
    {
        public string NoteSaveFilePath => GetPathForFile("notes.dat");

        public Dictionary<string, string> Notes = new Dictionary<string, string>();

        public override void OnStartup()
        {
            if (!File.Exists(NoteSaveFilePath))
                return;

            byte[] data = File.ReadAllBytes(NoteSaveFilePath);

            BitCompressor.FillObj(Notes, data);

        }
        public override void OnShutDown()
        {
            byte[] data = BitCompressor.GetBytes(Notes);
            File.WriteAllBytes(NoteSaveFilePath, data);
        }

        public void SetNote(string key, string data)
        {
            Notes[key] = data;
        }
        public string GetNote(string key)
        {
            if (Notes.TryGetValue(key, out string value))
            {
                return value;
            }

            return null;
        }

    }
}
