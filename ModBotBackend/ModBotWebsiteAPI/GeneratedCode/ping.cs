/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: ping
| |	Arguments: []
| |	ArgumentsInQuerystring: False
| |	HideInAPI: False
| |	MinimumAuthenticationLevelToCall: None
| |	ParseAsJson: False
| |
\*/

using System;
using System.Collections;

namespace ModBotWebsiteAPI
{
    public static partial class API
    {
        public static void Ping(Action<string> callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(_ping(callback));
        }

        private static IEnumerator _ping(Action<string> callback)
        {

            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "ping";
            JsonConstructor json = new JsonConstructor();
            data = json.ToString();

            yield return SendRequest(url, data, callback);
        }
    }
}
