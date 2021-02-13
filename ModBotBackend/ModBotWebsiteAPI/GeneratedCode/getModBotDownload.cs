/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: getModBotDownload
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
        public static void GetModBotDownload(Action<string> callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(_getModBotDownload(callback));
        }

        private static IEnumerator _getModBotDownload(Action<string> callback)
        {

            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "getModBotDownload";
            JsonConstructor json = new JsonConstructor();
            data = json.ToString();

            yield return SendRequest(url, data, callback);
        }
    }
}
