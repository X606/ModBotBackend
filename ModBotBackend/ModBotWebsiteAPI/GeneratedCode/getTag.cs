/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: getTag
| |	Arguments: [tagID]
| |	ArgumentsInQuerystring: True
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
        public static void GetTag(string tagID, Action<JsonObject> callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(_getTag(tagID, callback));
        }

        private static IEnumerator _getTag(string tagID, Action<JsonObject> callback)
        {

            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "getTag&tagID=" + tagID;
            data = "{}";


            yield return SendRequest(url, data, callback);
        }
    }
}
