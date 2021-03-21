using ModBotBackend.Managers;
using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UnityEngine;

namespace ModBotBackend.Operations
{
    [Operation("onFrontendPushed")]
    public class OnFrontendPushedOperation : OperationBase
    {
        public override string[] Arguments => new string[] { };

        public override bool ParseAsJson => false;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
        
        public override bool HideInAPI => true;

        public override bool ArgumentsInQuerystring => false;

        public override void OnOperation(HttpListenerContext context, Authentication authentication)
        {
            string key = Utils.GenerateSecureKey();

            NotesManager.Instance.SetNote(key, "Operation still loading....");

            HttpStream stream = new HttpStream(context.Response);
            stream.Send("Link: https://modbot.org/api?operation=noteOperation&key=" + key);
            stream.Close();

            string output;
            try
            {
                output = OnFrontendPushed();

            }
            catch (Exception e)
            {
                output = "Error!\n" + e.ToString();
            }

            NotesManager.Instance.SetNote(key, output);
        }

        public string OnFrontendPushed()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            byte[] result;
            using (var client = new HttpClient())
            {
                var response = client.GetAsync("http://github.com/X606/ModBotWebsite/archive/master.zip").Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    result = responseContent.ReadAsByteArrayAsync().Result;
                }
                else
                {
                    return "Error! StatusCode: \"" + response.StatusCode + "\"";
                }
            }

            string zipFilePath = Path.GetTempPath() + "latestSiteBuild.zip";
            string extractedPath = Path.GetTempPath() + "latestSiteBuild";
            if (Directory.Exists(extractedPath))
            {
                Directory.Delete(extractedPath, true);
            }

            File.WriteAllBytes(zipFilePath, result);

            Directory.CreateDirectory(extractedPath);

            ZipFile.ExtractToDirectory(zipFilePath, extractedPath);

            string contentPath = Directory.GetDirectories(extractedPath)[0];

            string websitePath = WebsiteRequestProcessor.GetWebsiteFilePath();

            Directory.Delete(websitePath, true);
            Directory.CreateDirectory(websitePath);

            CopyFilesRecursively(new DirectoryInfo(contentPath), new DirectoryInfo(websitePath));

            stopwatch.Stop();

            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            return "/// Updated frontend ///\n\n(this operation took " + (elapsedMilliseconds / 1000f) + " seconds)";
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }

    }
}
