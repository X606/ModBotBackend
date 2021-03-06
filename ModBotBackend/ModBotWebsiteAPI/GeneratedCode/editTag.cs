/*----------------------------------------------------------------------------------------------+
|                                                                                               |
|     This code was generated by the ModBotWebistieAPICodeGenerator, do not change it directly  |
|                                                                                               |
| /---------------------------------------------------------------------------------------------+
| |
| | Operation Options:
| |
| |	OperationID: editTag
| |	Arguments: [tagID, body]
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
        public static void EditTag(string tagID, string body, Action<JsonObject> callback) {
            StaticCoroutineRunner.StartStaticCoroutine(_editTag(tagID, body, callback));
        }

        private static IEnumerator _editTag(string tagID, string body, Action<JsonObject> callback) {
            
            string url = MODBOT_API_URL_BASE;
            string data = "";

            url += "editTag";
			JsonConstructor json = new JsonConstructor();
			json.AppendValue("tagID", tagID);
			json.AppendValue("body", body);
			data = json.ToString();

            yield return SendRequest(url, data, callback);
        }
    }
}
