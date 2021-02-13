using System.Net;

namespace ModBotBackend
{
    public abstract class RawDataOperationBase : OperationBase
    {
        public override void OnOperation(HttpListenerContext context, Authentication authentication)
        {
            Arguments arguments = GetArguments(context);

            ContentType = "application/octet-stream";
            StatusCode = HttpStatusCode.OK;

            byte[] data;
            if (arguments != null)
            {
                data = OnOperation(arguments, authentication);

                context.Response.StatusCode = (int)StatusCode;
                context.Response.ContentType = ContentType;
            }
            else
            {
                StatusCode = HttpStatusCode.BadRequest;
                data = new byte[0];
            }

            context.Response.ContentLength64 += data.Length;
            context.Response.OutputStream.Write(data, 0, data.Length);
            context.Response.Close();
        }

        public abstract byte[] OnOperation(Arguments arguments, Authentication authentication);

        protected string ContentType;
        protected HttpStatusCode StatusCode;
    }
}
