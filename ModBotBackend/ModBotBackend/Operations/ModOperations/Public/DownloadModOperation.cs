using ModBotBackend.Users;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace ModBotBackend.Operations
{
    [Operation("downloadMod")]
    public class DownloadModOperation : OperationBase
    {
        public override bool ParseAsJson => false;
        public override string[] Arguments => new string[] { "id" };
        public override string OverrideAPICallJavascript => "window.open(\"/api/?operation=downloadMod&id=\" + id);";
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override byte[] GetResponseForError(Exception e, out string contentType)
        {
            contentType = "text/plain";
            return Encoding.UTF8.GetBytes("ERROR!\nError:\n" + e.ToString());
        }

        public override void OnOperation(HttpListenerContext context, Authentication authentication)
        {
            string id = context.Request.QueryString["id"];

            if (!UploadedModsManager.Instance.HasModWithIdBeenUploaded(id))
            {
                Utils.SendErrorPage(context.Response, "No mod with the id \"" + id + "\" has been uploaded", true, HttpStatusCode.NotFound);
                return;
            }

            string path = UploadedModsManager.Instance.GetZippedModPathFromID(id);

            byte[] data = File.ReadAllBytes(path);

            string displayName = UploadedModsManager.Instance.GetModInfoFromId(id).DisplayName + ".zip";
            displayName = displayName.Replace(' ', '_');
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                displayName = displayName.Replace(c, '_');
            }

            SpecialModData specialModData = UploadedModsManager.Instance.GetSpecialModInfoFromId(id);
            specialModData.Downloads++;
            specialModData.Save();

            HttpStream httpStream = new HttpStream(context.Response);
            httpStream.SendFile(data, displayName);
            httpStream.Close();
        }

    }
}
