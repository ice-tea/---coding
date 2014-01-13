using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver.Util
{
    public static class Message
    {
        private static StringBuilder errorMsg = new StringBuilder();
        public static string ErrorMsg { get { return errorMsg.ToString(); } }

        private static StringBuilder warningMsg = new StringBuilder();
        public static string WarningMsg { get { return warningMsg.ToString(); } }

        private static StringBuilder msg = new StringBuilder();
        public static string Msg { get { return msg.ToString(); } }

        public static void ErrorAppend(string error) { errorMsg.Append(error); }
        public static void ErrorAppendLine(string error) { errorMsg.Append(error).Append('\n'); }
        public static void ErrorClear() { errorMsg.Clear(); }

        public static void WarningAppend(string warning) { warningMsg.Append(warning); }
        public static void WarningAppendLine(string warning) { warningMsg.Append(warning).Append('\n'); }
        public static void WarningClear() { warningMsg.Clear(); }

        public static void MessageAppend(string message) { msg.Append(message); }
        public static void MessageAppendLine(string message) { msg.Append(message).Append('\n'); }
        public static void MessageClear() { msg.Clear(); }
    }
}
