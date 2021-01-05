using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using ModBotBackend.Users;

namespace ModBotBackend
{
	public abstract class OperationBase
	{
		public abstract void OnOperation(HttpListenerContext context, Authentication authentication);

	}
}
