using ModBotBackend.Users;
using System;
using System.IO;
using System.Text;

namespace ModBotBackend.Operations.AdminOnly.AdminCommands
{
    [AdminCommand("logs")]
    public class LogsCommand : AdminCommand
    {
        public override void ProcessCommand(string[] arguments, Authentication authentication, User caller)
        {
            if (arguments.Length < 1)
            {
                OutputConsole.WriteLine("The logs command takes at least one argument, try \"logs list\" and \"logs get [log]\"");
                return;
            }


            if (arguments[0] == "list")
            {
                float daysBack = 2;
                if (arguments.Length >= 2)
                {
                    if (!float.TryParse(arguments[1], out daysBack))
                    {
                        daysBack = 2;
                    }
                }

                StringBuilder builder = new StringBuilder();
                builder.Append("Saved logs:\n");
                string[] logs = Directory.GetFiles(LogsManager.Instance.FolderPath);
                for (int i = 0; i < logs.Length; i++)
                {
                    FileInfo info = new FileInfo(logs[i]);

                    if (info.CreationTime.AddDays(daysBack) >= DateTime.Now)
                    {
                        builder.Append(info.Name + " (" + GetByteAmountWithUnit(info.Length) + ")");

                        if (i != (logs.Length - 1))
                            builder.Append('\n');
                    }

                }
                OutputConsole.WriteLine(builder.ToString());
                return;
            }
            else if (arguments[0] == "get")
            {
                if (arguments.Length < 2)
                {
                    OutputConsole.WriteLine("The paramiter \"get\" on the logs command takes 1 more argument");
                    return;
                }
                string log = arguments[1];
                string path = LogsManager.Instance.FolderPath + log;
                if (!File.Exists(path))
                {
                    OutputConsole.WriteLine("The log \"" + log + "\" could not be found");
                    return;
                }

                if (TemporaryFilesMananger.Instance.CreateTemporaryFile(path, out string key))
                {
                    string download = "<a href='/api/?operation=downloadTempFile&key=" + key + "'>here</a>";
                    OutputConsole.WriteLine("Download the log " + download + " (link valid for 30 seconds)", true);
                }
                else
                {
                    OutputConsole.WriteLine("Something went wrong when trying to read the log...");
                }

            }

        }


        static string GetByteAmountWithUnit(long byteAmount)
        {
            if (byteAmount < 1024)
                return byteAmount.ToString() + " bytes";

            byteAmount = byteAmount / 1024;
            if (byteAmount < 1024)
                return byteAmount.ToString() + "KB";

            byteAmount = byteAmount / 1024;
            if (byteAmount < 1024)
                return byteAmount.ToString() + "MB";

            byteAmount = byteAmount / 1024;
            if (byteAmount < 1024)
                return byteAmount.ToString() + "GB";

            byteAmount = byteAmount / 1024;
            return byteAmount.ToString() + "TB";
        }

    }
}
