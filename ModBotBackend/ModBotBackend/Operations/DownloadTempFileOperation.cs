using ModBotBackend.Users;
using System;
using System.Net;
using System.Text;

namespace ModBotBackend.Operations
{
    [Operation("downloadTempFile")]
    public class DownloadTempFileOperation : OperationBase
    {
        public override bool ParseAsJson => true;
        public override string[] Arguments => new string[] { "key" };
        public override string OverrideAPICallJavascript => "window.open(\"/api/?operation=downloadTempFile&key=\" + key);";
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override byte[] GetResponseForError(Exception e, out string contentType)
        {
            contentType = "text/plain";

            return Encoding.UTF8.GetBytes("ERROR!\nError:\n" + e.ToString());
        }

        public override void OnOperation(HttpListenerContext context, Authentication authentication)
        {
            string key = context.Request.QueryString["key"];

            if (!TemporaryFilesMananger.Instance.TryGetTempFile(key, out byte[] data, out string filename))
            {
                Utils.SendErrorPage(context.Response, "The requested file does not exist", true, HttpStatusCode.NotFound);
                return;
            }

            HttpStream httpStream = new HttpStream(context.Response);
            httpStream.SendFile(data, filename.Replace(" ", "_"));
            httpStream.Close();
        }

    }
}
