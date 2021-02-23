using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations
{
    [Operation("getMyIp")]
    public class GetMyIpOperation : OperationBase
    {
        public override string[] Arguments => new string[] { };

        public override bool ParseAsJson => false;

        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override void OnOperation(HttpListenerContext context, Authentication authentication)
        {
            context.Response.ContentType = "text/plain";
            HttpStream stream = new HttpStream(context.Response);
            stream.Send(authentication.IP);
            stream.Close();
        }
    }
}
