using ModBotBackend.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations
{
	[Operation("getAPI")]
	public class GetJavascriptAPIOperation : OperationBase
	{
		public override string[] Arguments => new string[] { };
		public override bool ParseAsJson => false;
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			StringBuilder javascriptBuilder = new StringBuilder();

			javascriptBuilder.Append("//AutenticationLevel = " + authentication.AuthenticationLevel.ToString());
			javascriptBuilder.Append("\n\n");

			javascriptBuilder.Append(Properties.Resources.APITemplate);

			foreach (KeyValuePair<string, OperationBase> item in Program.Operations)
			{
				if (item.Value.HideInAPI)
					continue;

				if (!authentication.HasAtLeastAuthenticationLevel(item.Value.MinimumAuthenticationLevelToCall))
					continue;

				javascriptBuilder.Append("API." + item.Key + " = function(");
				string[] arguments = item.Value.Arguments;

				for (int i = 0; i < arguments.Length; i++)
				{
					javascriptBuilder.Append(arguments[i]);

					if (i != (arguments.Length - 1))
						javascriptBuilder.Append(',');
				}

				javascriptBuilder.Append(") { ");
				string overrideJavascript = item.Value.OverrideAPICallJavascript;
				if (overrideJavascript == null)
				{
					javascriptBuilder.Append(" return new Promise(async resolve => { ");
					javascriptBuilder.Append("let e = await Post(\"/api?operation=");
					javascriptBuilder.Append(item.Key);
					if (item.Value.ArgumentsInQuerystring)
					{
						for (int i = 0; i < arguments.Length; i++)
						{
							javascriptBuilder.Append('&');


							javascriptBuilder.Append(arguments[i]);
							javascriptBuilder.Append("=\" + ");
							javascriptBuilder.Append(arguments[i]);

							if (i != (arguments.Length - 1))
								javascriptBuilder.Append(" + \"");
						}
					}
					else
					{
						javascriptBuilder.Append('\"');
					}

					javascriptBuilder.Append(",{ ");
					if (!item.Value.ArgumentsInQuerystring)
					{
						for (int i = 0; i < arguments.Length; i++)
						{
							javascriptBuilder.Append(arguments[i]);
							javascriptBuilder.Append(":");
							javascriptBuilder.Append(arguments[i]);

							if (i != (arguments.Length - 1))
								javascriptBuilder.Append(',');
						}
					}

					javascriptBuilder.Append("}); ");

					if (item.Value.ParseAsJson)
						javascriptBuilder.Append("e = JSON.parse(e); ");

					string overrideResolveJavascript = item.Value.OverrideResolveJavascript;
					if (overrideResolveJavascript == null)
					{
						javascriptBuilder.Append("resolve(e); ");
					} else
					{
						javascriptBuilder.Append(overrideResolveJavascript);
					}
					

					javascriptBuilder.Append("}); ");
				} else
				{
					javascriptBuilder.Append(overrideJavascript);
				}
				javascriptBuilder.Append("}; ");
			}

			javascriptBuilder.Append("export { API };");

			context.Response.ContentType = "application/javascript";
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(javascriptBuilder.ToString());
			httpStream.Close();
		}
	}
}
