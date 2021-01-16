/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: likeComment
| |	Arguments: [modId, commentId, likeState]
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
        public static void LikeComment(string modId, string commentId, string likeState, Action<JsonObject> callback) {
            StaticCoroutineRunner.StartStaticCoroutine(_likeComment(modId, commentId, likeState, callback));
        }

        private static IEnumerator _likeComment(string modId, string commentId, string likeState, Action<JsonObject> callback) {
            
            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "likeComment";
			JsonConstructor json = new JsonConstructor();
			json.AppendValue("modId", modId);
			json.AppendValue("commentId", commentId);
			json.AppendValue("likeState", likeState);
			data = json.ToString();

            yield return SendRequest(url, data, callback);
        }
    }
}