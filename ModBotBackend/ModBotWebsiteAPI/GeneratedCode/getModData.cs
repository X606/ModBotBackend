/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: getModData
| |	Arguments: [id]
| |	ArgumentsInQuerystring: True
| |	HideInAPI: False
| |	MinimumAuthenticationLevelToCall: None
| |	ParseAsJson: True
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
        public static void GetModData(string id, Action<JsonObject> callback) {
            StaticCoroutineRunner.StartStaticCoroutine(_getModData(id, callback));
        }

        private static IEnumerator _getModData(string id, Action<JsonObject> callback) {
            
            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "getModData&id=" + id;
			data = "{}";


            yield return SendRequest(url, data, callback);
        }
    }
}
