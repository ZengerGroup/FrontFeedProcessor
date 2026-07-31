using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrontFeedProcessor
{
    public static class Logger
    {
        static string LogDirectory;
        static string LogPath; //= Path.Combine(Configurator.LogPath, String.Format("{0}.txt", DateTime.Now.ToString("MMMyyyy")));
        static string CrashPath;// = Path.Combine(Configurator.LogPath, String.Format("Crash_{0}.txt", DateTime.Now.ToString("MMMyyyy")));

        public static void InitializeLogger(IConfiguration configuration)
        {
            //LogDirectory = (configuration.GetSection("Settings").Get<AppSettings>() ?? new AppSettings()).LogPath;
            LogDirectory = Preferences.Default.Get("log_path", @"C:\FrontFeedProcessor\Logs");
            Directory.CreateDirectory(LogDirectory);
            LogPath = Path.Combine(LogDirectory, String.Format("{0}.txt", DateTime.Now.ToString("MMMyyyy")));
            CrashPath = Path.Combine(LogDirectory, String.Format("Crash_{0}.txt", DateTime.Now.ToString("MMMyyyy")));
        }
        public static void WriteLog(string message, bool timestamp, params string[] messageArgs)
        {
            message = String.Format(message, messageArgs);
            message += Environment.NewLine;
            int writeAttempts = 0;
            while (writeAttempts < 3)
            {
                try
                {
                    File.AppendAllText(LogPath, (timestamp) ? String.Format("{0}: {1}", DateTime.Now.ToString("F"), message) : message);
                    break;
                }
                catch { writeAttempts++; }
            }
            if (writeAttempts == 3)
            {
                string errorPath = Path.Combine(LogDirectory, String.Format("{1}.txt", DateTime.Now.ToString("fffffff")));
                File.AppendAllText(errorPath, String.Format("UNABLE TO ACCESS LOG\n{0}", message));
            }
        }
        public static void ErrorExit(string[] message, int code)
        {
            WriteLog(message[0], true);
            string longMessage = "";
            for (int i = 0; i < message.Length; i++)
            {
                longMessage += message[i];
                if (i != message.Length - 1) longMessage += Environment.NewLine;
            }
            File.AppendAllText(CrashPath, String.Format(
                "*****START*****" + Environment.NewLine +
                "*****{0}*****" + Environment.NewLine +
                "{1}" + Environment.NewLine +
                "******END******", DateTime.Now.ToString("s"), longMessage));
            Environment.Exit(1);
        }
        public static void Display(string message, bool timestamp, params string[] messageArgs)
        {
            Console.WriteLine(message, messageArgs);
            Logger.WriteLog(message, timestamp, messageArgs);
        }
    }
}
