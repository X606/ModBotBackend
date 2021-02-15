using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend
{
    public static class ShutdownHandlerOverrider
    {
        public static void Init()
        {
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);
        }

        static void OnCloseApplication()
        {
            Console.WriteLine("Program being closed...");
            
            Program.OnProcessExit();

            Console.WriteLine("Done cleaning up!");

#if LOCAL
            System.Threading.Thread.Sleep(200);
#endif
        }
        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                OnCloseApplication();
            }
            return false;
        }

        static bool _shouldKeepRunning = true;
        public static bool ShouldKeepRunning => _shouldKeepRunning;

        #region unmanaged
        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
                                               // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        #endregion
    }
}
