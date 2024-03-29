﻿using System;
using System.Net;
using System.Text;

namespace ModBotBackend
{
    public abstract class PlainTextOperationBase : OperationBase
    {

        public override byte[] OnUnauthorized(Authentication authentication, out string contentType)
        {
            contentType = "text/plain";
            return Encoding.UTF8.GetBytes("Unauthorized");
        }

        public override void OnOperation(HttpListenerContext context, Authentication authentication)
        {
            Arguments arguments = GetArguments(context);

            ContentType = "text/plain";
            StatusCode = HttpStatusCode.OK;
            string text;
            if (arguments != null)
            {
                text = OnOperation(arguments, authentication);

                context.Response.ContentType = ContentType;
                context.Response.StatusCode = (int)StatusCode;

                if (AllowCaching)
                    Utils.AddCacheHeaders(context.Response);

            }
            else
            {
                text = "The provided request was invalid";
                StatusCode = HttpStatusCode.BadRequest;
            }

            HttpStream stream = new HttpStream(context.Response);
            stream.Send(text);
            stream.Close();
        }

        public override byte[] GetResponseForError(Exception e, out string contentType)
        {
            contentType = "text/plain";

            string errorMessage = "ERROR!\nError:\n" + e.ToString();

            return Encoding.UTF8.GetBytes(errorMessage);
        }

        public abstract string OnOperation(Arguments arguments, Authentication authentication);

        protected string ContentType;
        protected HttpStatusCode StatusCode;

    }
}
