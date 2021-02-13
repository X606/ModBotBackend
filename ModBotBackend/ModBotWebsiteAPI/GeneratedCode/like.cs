/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: like
| |	Arguments: [likedModId, likeState]
| |	ArgumentsInQuerystring: False
| |	HideInAPI: False
| |	MinimumAuthenticationLevelToCall: BasicUser
| |	ParseAsJson: True
| |
\*/

using System;
using System.Collections;

namespace ModBotWebsiteAPI
{
    public static partial class API
    {
        public static void Like(string likedModId, string likeState, Action<JsonObject> callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(_like(likedModId, likeState, callback));
        }

        private static IEnumerator _like(string likedModId, string likeState, Action<JsonObject> callback)
        {

            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "like";
            JsonConstructor json = new JsonConstructor();
            json.AppendValue("likedModId", likedModId);
            json.AppendValue("likeState", likeState);
            data = json.ToString();

            yield return SendRequest(url, data, callback);
        }
    }
}
