/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: getCurrentUser
| |	Arguments: []
| |	ArgumentsInQuerystring: False
| |	HideInAPI: False
| |	MinimumAuthenticationLevelToCall: BasicUser
| |	ParseAsJson: False
| |
\*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotWebsiteAPI
{
    public static partial class API
    {
        public static void GetCurrentUser(Action<string> callback) {
            StaticCoroutineRunner.StartStaticCoroutine(_getCurrentUser(callback));
        }

        private static IEnumerator _getCurrentUser(Action<string> callback) {
            
            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "getCurrentUser";
			JsonConstructor json = new JsonConstructor();
			data = json.ToString();

            yield return SendRequest(url, data, callback);
        }
    }
}
