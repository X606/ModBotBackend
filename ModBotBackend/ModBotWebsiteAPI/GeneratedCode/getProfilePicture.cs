/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: getProfilePicture
| |	Arguments: [element, id]
| |	ArgumentsInQuerystring: True
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
        public static void GetProfilePicture(string element, string id, Action<string> callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(_getProfilePicture(element, id, callback));
        }

        private static IEnumerator _getProfilePicture(string element, string id, Action<string> callback)
        {

            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "getProfilePicture&element=" + element + "&id=" + id;
            data = "{}";


            yield return SendRequest(url, data, callback);
        }
    }
}
