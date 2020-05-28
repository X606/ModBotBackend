using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations
{
	public class TestOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context)
		{
			context.Response.Redirect("https://clonedronemodbot.com/error.html?error=test works&notError=true");
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send("re-routed");
			httpStream.Close();
		}

	}
}
