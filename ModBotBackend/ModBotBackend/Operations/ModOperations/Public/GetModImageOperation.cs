using ModBotBackend.Users;
using ModLibrary;
using System;
using System.Collections.Concurrent;
using System.Drawing;

namespace ModBotBackend.Operations
{

    [Operation("getModImage")]
    public class GetModImageOperation : RawDataOperationBase
    {
        public override bool ParseAsJson => false;
        public override string[] Arguments => new string[] { "element", "id" };
        public override bool ArgumentsInQuerystring => true;
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
        public override string OverrideAPICallJavascript => "let destroy = () => { element.src = \"/api/?operation=getModImage&size=\" + element.clientWidth + \"x\" + element.clientHeight + \"&id=\" + id; }; if(element.clientWidth == 0 || element.clientHeight == 0) { setTimeout(destroy,100); } else { destroy(); }";

        static ConcurrentDictionary<string, byte[]> _rescaledImageCache = new ConcurrentDictionary<string, byte[]>();

        public override byte[] GetResponseForError(Exception e, out string contentType)
        {
            contentType = "image/png";

            ImageConverter converter = new ImageConverter();

            byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
            return data;
        }

        public override byte[] OnOperation(Arguments arguments, Authentication authentication)
        {
            ContentType = "image/png";

            string id = arguments["id"];

            if (!UploadedModsManager.Instance.HasModWithIdBeenUploaded(id))
            {
                ImageConverter converter = new ImageConverter();
                int statusCode = 0;
                byte[] data = WebsiteRequestProcessor.OnRequest("Assets/DefaultAvatar.png", out string contentType, ref statusCode);
                return data;
            }

            ModInfo modInfo = UploadedModsManager.Instance.GetModInfoFromId(id);

            if (!modInfo.HasImage)
            {
                ImageConverter converter = new ImageConverter();

                byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
                return data;
            }


            string imageFilePath = UploadedModsManager.Instance.GetModFolderPath(id) + modInfo.ImageFileName;

            string size = arguments["size"];
            if (size == null)
            {
                size = "64x64";
            }

            if (!ImageResizer.TryScaleImageAndGetAsByteArray(arguments["id"], size, imageFilePath, _rescaledImageCache, out byte[] imgData))
            {
                ImageConverter converter = new ImageConverter();

                byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
                return data;
            }


            return imgData;
        }

    }
}
