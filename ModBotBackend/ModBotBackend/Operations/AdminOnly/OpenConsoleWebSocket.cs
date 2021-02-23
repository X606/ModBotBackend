using ModBotBackend.Users;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace ModBotBackend.Operations.AdminOnly
{
    [Operation("openConsoleWebSocket")]
    public class OpenConsoleWebSocket : OperationBase
    {
        public override bool ParseAsJson => false;
        public override string[] Arguments => new string[] { };
        public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.Admin;
        public override string OverrideAPICallJavascript => "return new WebSocket((location.protocol == \"https:\" ? \"wss:\" : \"ws:\") + \"//\" + location.host + \"/api?operation=openConsoleWebSocket\");";

        public override byte[] GetResponseForError(Exception e, out string contentType)
        {
            string error = "ERROR!\nError:\n" + e.ToString();

            contentType = "text/plain";
            return Encoding.UTF8.GetBytes(error);
        }

        public override void OnOperation(HttpListenerContext context, Authentication authentication)
        {
            if (!context.Request.IsWebSocketRequest)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                HttpStream httpStream = new HttpStream(context.Response);
                httpStream.Send("<html><body>websocket only!</body></html>");
                httpStream.Close();
            }

            if (!authentication.HasAtLeastAuthenticationLevel(Users.AuthenticationLevel.Admin))
            {
                HttpListenerWebSocketContext webNoSocket = context.AcceptWebSocketAsync(null).ConfigureAwait(true).GetAwaiter().GetResult();
                byte[] buffer = Encoding.UTF8.GetBytes("You do not have the requierd permissions to use this.");
                webNoSocket.WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, System.Threading.CancellationToken.None).ConfigureAwait(true).GetAwaiter().GetResult();
                webNoSocket.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "You do not have the requierd permissions to use this.", System.Threading.CancellationToken.None).ConfigureAwait(true).GetAwaiter().GetResult();
                webNoSocket.WebSocket.Dispose();
                return;
            }

            Task.Factory.StartNew(() => handleWebSocket(context, authentication));
        }

        static async Task handleWebSocket(HttpListenerContext context, Authentication authentication)
        {
            User user = UserManager.Instance.GetUserFromId(authentication.UserID);

            HttpListenerWebSocketContext websocket = await context.AcceptWebSocketAsync(null);

            Action<string, bool> onWriteLine = null;
            onWriteLine = delegate (string line, bool dontHttpEncode)
            {
                if (websocket == null || websocket.WebSocket.State != WebSocketState.Open)
                {
                    Console.WriteLine("error, websocket is not connected");
                    return;
                }

                if (!dontHttpEncode)
                {
                    line = System.Web.HttpUtility.HtmlEncode(line);
                }

                line = line.Replace("\n", "<br>");

                byte[] buffer = Encoding.UTF8.GetBytes(line);

                websocket.WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, System.Threading.CancellationToken.None).ConfigureAwait(true).GetAwaiter().GetResult();
            };

            OutputConsole.OnWriteLine += onWriteLine;

            OutputConsole.WriteLine(user.Username + " connected to console!");

            CancellationTokenSource token = new CancellationTokenSource();
            DateTime lastHeartBeat = DateTime.Now;

            Task.Factory.StartNew(async delegate
            {
                while (true)
                {
                    if (websocket == null)
                        return;

                    byte[] heartbeatBuffer = new byte[16];
                    await websocket.WebSocket.ReceiveAsync(new ArraySegment<byte>(heartbeatBuffer, 0, heartbeatBuffer.Length), token.Token);
                    lastHeartBeat = DateTime.Now;
                }
            }, token.Token);
            Task.Factory.StartNew(async delegate
            {
                while (true)
                {
                    await Task.Delay(1000);

                    TimeSpan time = DateTime.Now - lastHeartBeat;

                    if (time.TotalSeconds > 3)
                    {
                        OutputConsole.OnWriteLine -= onWriteLine;
                        websocket.WebSocket.Abort();
                        websocket.WebSocket.Dispose();
                        websocket = null;
                        token.Cancel();

                        OutputConsole.WriteLine(user.Username + " disconnected (Connection timed out)");
                        return;
                    }
                }
            }, token.Token);

        }


    }
}
