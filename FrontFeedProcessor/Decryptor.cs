using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FrontFeedProcessor
{
    public class Decryptor
    {
        public string OutputPath;
        public bool Success;
        public Decryptor(string filePath)
        {
            OutputPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
            Logger.WriteLog("Writing to: {0}", false, OutputPath);
            Success = true;
            try
            {
                RunGPG(filePath);
                Success = true;
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.Message, false);
                Success = false;
            }
        }
        private void RunGPG(string filePath)
        {
            Logger.WriteLog("Running GPG.", false);
            Process gpg = new Process();
            ProcessStartInfo gpgInfo = new ProcessStartInfo();
            gpgInfo.FileName = Preferences.Default.Get("gpg_path", "Unknown");
            gpgInfo.RedirectStandardInput = true;
            string arguments = String.Format("--ignore-mdc-error --pinentry-mode loopback --passphrase-fd 0 -o {0} -d {1}", OutputPath, filePath);
            Logger.WriteLog(arguments, false);
            gpgInfo.Arguments = arguments;
            gpgInfo.UseShellExecute = false;
            gpg.StartInfo = gpgInfo;
            gpg.Start();
            using (StreamWriter sw = gpg.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine(Preferences.Default.Get("secret_string", "None"));
                }
            }
            gpg.WaitForExit();
        }
    }
}
