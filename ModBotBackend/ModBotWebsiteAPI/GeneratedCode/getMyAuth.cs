/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: getMyAuth
| |	Arguments: []
| |	ArgumentsInQuerystring: False
| |	HideInAPI: False
| |	MinimumAuthenticationLevelToCall: None
| |	ParseAsJson: True
| |
\*/

using System;
using System.Collections;

namespace ModBotWebsiteAPI
{
    public static partial class API
    {
        public static void GetMyAuth(Action<JsonObject> callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(_getMyAuth(callback));
        }

        private static IEnumerator _getMyAuth(Action<JsonObject> callback)
        {

            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "getMyAuth";
            JsonConstructor json = new JsonConstructor();
            data = json.ToString();

            yield return SendRequest(url, data, callback);
        }
    }
}
