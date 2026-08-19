using System;
using System.Collections.Generic;
using System.Text;

namespace FrontFeedProcessor
{
    public static class ErrorReport
    {
        public static List<string> Messages;
        public static void Initialize()
        {
            Messages = new List<string>();
        }
        public static void NewError(string message)
        {
            Messages.Add(message);
            Logger.WriteLog(message, false);
        }
    }
}
