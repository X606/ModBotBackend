using ModBotBackend.Users;
using ModBotBackend.Users.Sessions;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace ModBotBackend
{
    static class Program
    {
        static ConcurrentBag<string> TrackedIps = new ConcurrentBag<string>();

        static void Main(string[] args)
        {
            ShutdownHandlerOverrider.Init();

            PopulateOperations();
            PopulateOwnFolderObjects();

            Directory.CreateDirectory(BasePath);

            Directory.CreateDirectory(ModTemplateFilePath);
            Directory.CreateDirectory(WebsitePath);

            if (args.Contains("-httpOnly"))
            {
                HttpListener httpMainListener = new HttpListener();
                httpMainListener.Prefixes.Add("http://+:80/");
                httpMainListener.Start();
                listenMain(httpMainListener);

                OutputConsole.WriteLine("Listening...");
                OutputConsole.WriteLine("WARNING: You are currently running in http only mode, this is only intended for test use, do not do this in production.");

                while (ShutdownHandlerOverrider.ShouldKeepRunning)
                    Thread.Sleep(10);
            }


            HttpListener httpsListener = new HttpListener();
            //httpListener.Prefixes.Add("http://+:80/");
            httpsListener.Prefixes.Add("https://+:443/");
            httpsListener.Start();
            listenMain(httpsListener);

            OutputConsole.WriteLine("Listening...");

            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://+:80/");
            httpListener.Start();
            listenHttp(httpListener);

            while (ShutdownHandlerOverrider.ShouldKeepRunning)
                Thread.Sleep(10);
        }

        static async void listenMain(HttpListener httpListener)
        {
            while (true)
            {
                var context = await httpListener.GetContextAsync();
                //Console.WriteLine("Client connected");
                Task.Factory.StartNew(() => processRequest(context));
            }
        }
        static async void listenHttp(HttpListener httpListener)
        {
            while (true)
            {
                var context = await httpListener.GetContextAsync();
                //Console.WriteLine("Client connected");
                Task.Factory.StartNew(() =>
                {
                    UriBuilder builder = new UriBuilder(context.Request.Url);

                    builder.Scheme = "https";
                    builder.Port = 443;

                    string url = builder.Uri.ToString();

                    context.Response.Redirect(url);
                    HttpStream httpStream = new HttpStream(context.Response);
                    httpStream.Send("Re-rounted to https");
                    httpStream.Close();
                });
            }
        }

        static string GetCookie(HttpListenerRequest request, string cookieName)
        {
            for (int i = 0; i < request.Cookies.Count; i++)
            {
                if (request.Cookies[i].Name == cookieName)
                    return request.Cookies[i].Value;
            }

            return null;
        }

        static void processRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;

            string sessionID = GetCookie(request, "SessionID");

            string clientIP = context.Request.RemoteEndPoint.Address.ToString();

            Authentication authentication;
            if (SessionsManager.Instance.VerifyKey(sessionID, out Session session))
            {
                authentication = new Authentication(session.AuthenticationLevel, session.OwnerUserID, sessionID, clientIP);

                if (!TrackedIps.Contains(clientIP))
                {
                    TrackedIps.Add(clientIP);

                    User user = UserManager.Instance.GetUserFromId(session.OwnerUserID);

                    if (!user.Ips.Contains(clientIP))
                    {
                        user.Ips.Add(clientIP);
                        user.Save();
                    }
                }
            }
            else
            {
                authentication = new Authentication(AuthenticationLevel.None, "", "", clientIP);
            }

            if (authentication.IsHardBanned) // if the user is hard banned, just close the connection
            {
                context.Response.Abort();
                return;
            }

            string operation = request.QueryString.Get("operation");

            string absolutePath = request.Url.AbsolutePath;
            if (absolutePath == "/api/" || absolutePath == "/api")
            {
                if (operation != null && Operations.TryGetValue(operation, out OperationBase selectedOperation))
                {
                    try
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        bool isAllowedToCall = authentication.HasAtLeastAuthenticationLevel(selectedOperation.MinimumAuthenticationLevelToCall);

                        if (selectedOperation.AllowedForBannedUsers == OperationBase.BannedUserCallability.Never)
                            isAllowedToCall = false;

                        if (selectedOperation.AllowedForBannedUsers == OperationBase.BannedUserCallability.Default)
                        {
                            if (selectedOperation.MinimumAuthenticationLevelToCall != AuthenticationLevel.None && authentication.IsBanned)
                            {
                                isAllowedToCall = false;
                            }
                        }

                        if (isAllowedToCall)
                        {
                            selectedOperation.OnOperation(context, authentication);
                        }
                        else
                        {
                            byte[] data = selectedOperation.OnUnauthorized(authentication, out string contentType);

                            context.Response.ContentType = contentType;
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentLength64 += data.Length;
                            context.Response.OutputStream.Write(data, 0, data.Length);
                            context.Response.Close();
                        }

                        stopwatch.Stop();
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            byte[] data = selectedOperation.GetResponseForError(e, out string contentType);

                            context.Response.ContentType = contentType;
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentLength64 += data.Length;
                            context.Response.OutputStream.Write(data, 0, data.Length);
                            context.Response.Close();

                        }
                        catch
                        {
                            // At this point just forget it and move on
                            context.Response.Abort();
                            return;
                        }
                    }
                }
                else
                {
                    if (operation == null)
                        operation = "null";

                    Utils.SendErrorPage(context.Response, "invalid operation \"" + operation + "\"", true, HttpStatusCode.BadRequest);
                }
            }
            else
            {
                try
                {
                    WebsiteRequestProcessor.OnRequest(context);
                }
                catch (Exception e)
                {
#if DEBUG
                    //OutputConsole.WriteLine("\n" + e.ToString());
#endif
                }


            }
        }

        public static string BasePath => Directory.GetCurrentDirectory() + "/SiteData/";

        public static string ModTemplateFilePath => BasePath + "/ModTemplate/";
        public static string WebsitePath => BasePath + "/Website/";
        public static string WebsiteFile => BasePath + "/Website.txt";


        public static void PopulateOperations()
        {
            Operations.Clear();

            Type[] loadedTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            for (int i = 0; i < loadedTypes.Length; i++)
            {
                OperationAttribute operationAttribute = (OperationAttribute)Attribute.GetCustomAttribute(loadedTypes[i], typeof(OperationAttribute));
                if (operationAttribute == null)
                    continue;

                if (!ExtendsFrom(loadedTypes[i].BaseType, typeof(OperationBase)))
                    continue;

                Operations.Add(operationAttribute.OperationKey, (OperationBase)Activator.CreateInstance(loadedTypes[i]));
            }

        }

        public static bool ExtendsFrom(Type type, Type baseType)
        {
            while (type != null)
            {
                if (type == baseType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }

        public static void PopulateOwnFolderObjects()
        {
            OwnFolderObjects.Clear();

            Type[] loadedTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            for (int i = 0; i < loadedTypes.Length; i++)
            {
                FolderNameAttribute operationAttribute = (FolderNameAttribute)Attribute.GetCustomAttribute(loadedTypes[i], typeof(FolderNameAttribute));
                if (operationAttribute == null)
                    continue;

                object instance = Activator.CreateInstance(loadedTypes[i]);
                instance.GetType().GetMethod("Init").Invoke(instance, new object[] { operationAttribute.FolderName });
                OwnFolderObjects.Add(instance);
            }

        }

        public static void OnProcessExit()
        {
            for (int i = 0; i < OwnFolderObjects.Count; i++)
            {
                OwnFolderObjects[i].GetType().GetMethod("OnShutDown").Invoke(OwnFolderObjects[i], new object[] { });
            }

        }

        public static readonly Dictionary<string, OperationBase> Operations = new Dictionary<string, OperationBase>();
        public static readonly List<object> OwnFolderObjects = new List<object>();

    }

    [Serializable]
    public class InternalError
    {
        public InternalError(string _error)
        {
            error = _error;
        }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        public bool isError = true;
        public string error;
    }
}
