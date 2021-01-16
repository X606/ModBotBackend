/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: hasLikedComment
| |	Arguments: [modId, commentId]
| |	ArgumentsInQuerystring: False
| |	HideInAPI: False
| |	MinimumAuthenticationLevelToCall: BasicUser
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
        public static void HasLikedComment(string modId, string commentId, Action<JsonObject> callback) {
            StaticCoroutineRunner.StartStaticCoroutine(_hasLikedComment(modId, commentId, callback));
        }

        private static IEnumerator _hasLikedComment(string modId, string commentId, Action<JsonObject> callback) {
            
            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "hasLikedComment";
			JsonConstructor json = new JsonConstructor();
			json.AppendValue("modId", modId);
			json.AppendValue("commentId", commentId);
			data = json.ToString();

            yield return SendRequest(url, data, callback);
        }
    }
}