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

		public abstract string[] Arguments { get; }
		public abstract bool ParseAsJson { get; }
		public abstract AuthenticationLevel MinimumAuthenticationLevelToCall { get; }

		public virtual bool ArgumentsInQuerystring => false;
		public virtual string OverrideResolveJavascript => null;
		public virtual string OverrideAPICallJavascript => null;
		public virtual bool HideInAPI => false;
	}

	public class OperationAttribute : Attribute
	{
		public OperationAttribute(string operationKey)
		{
			OperationKey = operationKey;
		}
		public string OperationKey;
	}
}
