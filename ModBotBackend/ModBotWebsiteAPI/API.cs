using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace ModBotWebsiteAPI
{
    public static partial class API
    {
        public const string MODBOT_API_URL_BASE = "https://modbot.org/api?operation=";
        static string _sessionID = "";

        public static void SetSessionID(string sessionID)
        {
            _sessionID = sessionID;
        }
        public static bool HasSession
        {
            get
            {
                return _sessionID != "";
            }
        }

        internal static IEnumerator SendRequest(string url, string data, Action<string> callback)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url);
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.method = UnityWebRequest.kHttpVerbPOST;

            if (_sessionID != null)
                webRequest.SetRequestHeader("Cookie", "SessionID=" + _sessionID);

            yield return webRequest.SendWebRequest();

            callback(webRequest.downloadHandler.text);
            yield return null;
        }
        internal static IEnumerator SendRequest(string url, string data, Action<JsonObject> callback)
        {
            //UnityWebRequest webRequest = UnityWebRequest.Post(url, data);
            UnityWebRequest webRequest = new UnityWebRequest(url);
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.method = UnityWebRequest.kHttpVerbPOST;

            if (_sessionID != null)
                webRequest.SetRequestHeader("Cookie", "SessionID=" + _sessionID);

            yield return webRequest.SendWebRequest();

            callback(new JsonObject(webRequest.downloadHandler.text));
            yield return null;
        }
    }

    class JsonConstructor
    {
        StringBuilder _builder;
        public JsonConstructor()
        {
            _builder = new StringBuilder();
            _builder.Append('{');
            _appended = 0;
        }

        int _appended;

        public void AppendValue(string key, string value)
        {
            if (_appended != 0)
                _builder.Append(", ");

            key = key.Replace("\"", "\\\"");
            value = value.Replace("\"", "\\\"");

            _builder.Append("\"" + key + "\"");
            _builder.Append(":");
            _builder.Append("\"" + value + "\"");

            _appended++;
        }


        public override string ToString()
        {
            return _builder.ToString() + "}";
        }
    }

    public class JsonObject
    {
        Dictionary<string, object> _parsedData = new Dictionary<string, object>();
        public readonly string RawData;

        public object this[string data]
        {
            get
            {
                if (_parsedData.TryGetValue(data, out object value))
                {
                    return value;
                }

                return "";
            }
        }

        public JsonObject(string data)
        {
            RawData = data;

            _parsedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

            /*
			for (int i = 1; i < data.Length; i++)
			{
				string key = parseOutString(data, i, out i);
				i++;

				while (data[i] == ' ')
					i++;

				if (data[i] != ':')
				{
					throw new Exception("Expeced \":\", got \"" + data[i] + "\"");
				}
				i++;

				while (data[i] == ' ')
					i++;

				string value = "";

				if (data[i] == '\"')
				{
					value = parseOutString(data, i, out i);
					i++;
				} else
				{
					int startIndex = i;
					while (data[i] != ',' && data[i] != '}')
						i++;

					value = data.Substring(startIndex, i - startIndex);

				}

				_parsedData.Add(key, value);
				Console.WriteLine(key + ": " + value);
			}
			*/


        }

        static string parseOutString(string data, int startIndex, out int endIndex)
        {
            if (tryGetEndOfString(data, startIndex, out int end))
            {
                endIndex = end;
            }
            else
            {
                endIndex = 0;
                return "";
            }

            return data.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        static bool tryGetEndOfString(string data, int index, out int endIndex)
        {
            if (data[index] != '\"')
                throw new Exception("The passed index was not a start of a string, its was \"" + data[index] + "\"");

            bool nextCharacterSpecial = false;
            for (int i = index + 1; i < data.Length; i++)
            {
                if (!nextCharacterSpecial)
                {
                    if (data[i] == '\"')
                    {
                        endIndex = i;
                        return true;
                    }
                }


                nextCharacterSpecial = data[i] == '\\';
            }

            endIndex = -1;
            return false;
        }

    }
}
