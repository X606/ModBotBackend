/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: getAPI
| |	Arguments: []
| |	ArgumentsInQuerystring: False
| |	HideInAPI: False
| |	MinimumAuthenticationLevelToCall: None
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
        public static void GetAPI(Action<string> callback) {
            StaticCoroutineRunner.StartStaticCoroutine(_getAPI(callback));
        }

        private static IEnumerator _getAPI(Action<string> callback) {
            
            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "getAPI";
			JsonConstructor json = new JsonConstructor();
			data = json.ToString();

            yield return SendRequest(url, data, callback);
        }
    }
}
