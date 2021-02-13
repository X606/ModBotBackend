/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: postComment
| |	Arguments: [targetModId, commentBody]
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
        public static void PostComment(string targetModId, string commentBody, Action<JsonObject> callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(_postComment(targetModId, commentBody, callback));
        }

        private static IEnumerator _postComment(string targetModId, string commentBody, Action<JsonObject> callback)
        {

            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "postComment";
            JsonConstructor json = new JsonConstructor();
            json.AppendValue("targetModId", targetModId);
            json.AppendValue("commentBody", commentBody);
            data = json.ToString();

            yield return SendRequest(url, data, callback);
        }
    }
}
