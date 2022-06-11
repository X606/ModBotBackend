using HttpUtils;
using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations
{
    [Operation("PrintPostBody")]
    public class PrintPostBodyOperation : OperationBase
    {
        public override string[] Arguments => new string[] { };

        public override bool ParseAsJson => false;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override void OnOperation(HttpListenerContext context, Authentication authentication)
        {
            byte[] data = Misc.ToByteArray(context.Request.InputStream);
            string strData = Encoding.UTF8.GetString(data);

            context.Response.ContentType = "text/plain";

            HttpStream httpStream = new HttpStream(context.Response);
            httpStream.Send(strData);
            httpStream.Close();
        }
    }
}
