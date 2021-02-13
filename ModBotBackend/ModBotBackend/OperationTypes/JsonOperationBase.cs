using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend
{
    public abstract class JsonOperationBase : OperationBase
    {
        public override void OnOperation(HttpListenerContext context, Authentication authentication)
        {
            Arguments arguments = GetArguments(context);

            ContentType = "application/json";
            StatusCode = HttpStatusCode.OK;

            JsonOperationResponseBase operationResponseBase;
            if (arguments != null)
            {
                operationResponseBase = OnOperation(arguments, authentication);

                context.Response.ContentType = ContentType;
                context.Response.StatusCode = (int)StatusCode;
            }
            else
            {
                StatusCode = HttpStatusCode.BadRequest;
                operationResponseBase = new JsonOperationResponseBase()
                {
                    Error = "The provided request was invalid"
                };
            }

            if (operationResponseBase != null)
            {
                HttpStream stream = new HttpStream(context.Response);
                stream.Send(operationResponseBase.ToJson());
                stream.Close();
            }
            else
            {
                HttpStream stream = new HttpStream(context.Response);
                stream.Send("null");
                stream.Close();
            }
            
        }

        public override byte[] GetResponseForError(Exception e, out string contentType)
        {
            contentType = "application/json";

            string json = new JsonOperationResponseBase()
            {
                Error = e.ToString()
            }.ToJson();

            return Encoding.UTF8.GetBytes(json);
        }

        public abstract JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication);

        protected string ContentType;
        protected HttpStatusCode StatusCode;
    }

    public class JsonOperationResponseBase
    {
        public string Error;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static T FromJson<T>(string json) where T : JsonOperationResponseBase
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

}
