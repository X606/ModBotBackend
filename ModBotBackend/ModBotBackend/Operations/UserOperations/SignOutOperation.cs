using HttpUtils;
using ModBotBackend.Users;
using ModBotBackend.Users.Sessions;
using System;
using System.Net;
using System.Text;

namespace ModBotBackend.Operations
{
    [Operation("signOut")]
    public class SignOutOperation : OperationBase
    {
        public override bool ParseAsJson => true;
        public override string[] Arguments => new string[] { };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

        public override byte[] GetResponseForError(Exception e, out string contentType)
        {
            contentType = "application/json";

            string json = new SignOutResponse()
            {
                Error = e.ToString()
            }.ToJson();

            return Encoding.UTF8.GetBytes(json);
        }

        public override void OnOperation(HttpListenerContext context, Authentication authentication)
        {
            context.Response.ContentType = "application/json";

            byte[] data = Misc.ToByteArray(context.Request.InputStream);
            string json = Encoding.UTF8.GetString(data);

            if (!authentication.IsSignedIn)
            {
                HttpStream stream = new HttpStream(context.Response);
                stream.Send(new SignOutResponse()
                {
                    Error = "You are not signed in."
                }.ToJson());
                stream.Close();
                return;
            }

            SessionsManager.Instance.RemoveSession(authentication.SessionID);

            // Delete the sessionID cookie
            Cookie cookie = context.Request.Cookies["SessionID"];
            cookie.Expires = DateTime.Now.AddDays(-10);
            cookie.Value = null;
            context.Response.SetCookie(cookie);

            HttpStream httpStream = new HttpStream(context.Response);
            httpStream.Send(new SignOutResponse()
            {
                message = "signed out!"
            }.ToJson());
            httpStream.Close();
        }

        [Serializable]
        private class SignOutResponse
        {
            public string message;
            public string Error;

            public string ToJson()
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }
        }
    }
}
