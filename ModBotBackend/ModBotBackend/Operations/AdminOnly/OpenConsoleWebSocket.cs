using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using ModBotBackend.Users;

namespace ModBotBackend.Operations.AdminOnly
{
	[Operation("openConsoleWebSocket")]
	public class OpenConsoleWebSocket : OperationBase
	{
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

			User user = UserManager.GetUserFromId(authentication.UserID);

			HttpListenerWebSocketContext websocket = context.AcceptWebSocketAsync(null).ConfigureAwait(true).GetAwaiter().GetResult();
			Action<string> onWriteLine = null;
			onWriteLine = delegate (string line)
			{
				if (websocket.WebSocket.State != WebSocketState.Open)
				{
					OutputConsole.WriteLine("Cleaing up after " + user.Username + "s client disconected from console....");
					OutputConsole.OnWriteLine -= onWriteLine;
					websocket.WebSocket.Dispose();
					return;
				}

				byte[] buffer = Encoding.UTF8.GetBytes(line);

				websocket.WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, System.Threading.CancellationToken.None).ConfigureAwait(true).GetAwaiter().GetResult();
			};

			OutputConsole.OnWriteLine += onWriteLine;

			OutputConsole.WriteLine(user.Username + " connected to console!");
		}
	}
}
