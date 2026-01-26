#region Usings

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

#endregion

namespace TCR.Lib.SysLog
{
    public class SyslogSender
    {
        private const Facility Facility = SysLog.Facility.Local0;
        private const string Nilvalue = "-";

        /// <summary>
        ///     the protocol version
        /// </summary>
        private const int Version = 1;

        private static string _hostName;
        private static string _processId;
        private static string _processName;

        private static string HostName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_hostName))
                    _hostName = Dns.GetHostEntry(Environment.MachineName).HostName;
                return _hostName;
            }
        }

        private static string ProcessId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_processId))
                {
                    var process = Process.GetCurrentProcess();
                    _processId = process.Id.ToString();
                    _processName = process.ProcessName;
                }
                return _processId;
            }
        }

        private static string ProcessName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_processName))
                {
                    var process = Process.GetCurrentProcess();
                    _processId = process.Id.ToString();
                    _processName = process.ProcessName;
                }
                return _processName;
            }
        }

        private static byte[] ConstructMessage(string AppName, Level level, Facility facility, string messageId,
            string message = "", string ProcID = "")
        {
            if (!string.IsNullOrEmpty(message) && message.Length > 512)
            {
                message = message.Substring(0, 508) + "...";
            }

            var prival = ((int) facility)*8 + ((int) level);
            var pri = string.Format("<{0}>", prival);
            var timestamp =
                DateTime.Now.ToString(
                    "yyyy-MM-ddTHH:mm:ss.ffffffzzz");
            var appName = string.IsNullOrWhiteSpace(AppName) ? Nilvalue : AppName;
            var procId = string.IsNullOrWhiteSpace(ProcID) ? ProcessId : ProcID;
            var msgId = string.IsNullOrWhiteSpace(messageId) ? Nilvalue : messageId;

            var header = string.Format("{0}{1} {2} {3} {4} {5} {6}", pri, Version, timestamp, HostName, appName, procId,
                msgId);
            var sd = Nilvalue;

            var syslogMsg = new List<byte>();
            syslogMsg.AddRange(Encoding.ASCII.GetBytes(header));
            syslogMsg.AddRange(Encoding.ASCII.GetBytes(" "));
            syslogMsg.AddRange(Encoding.ASCII.GetBytes(sd));

            if (!string.IsNullOrWhiteSpace(message))
                message = message.Replace("\n", "").Replace("\r", "");

            if (!string.IsNullOrWhiteSpace(message))
            {
                syslogMsg.AddRange(Encoding.ASCII.GetBytes(" "));
                syslogMsg.AddRange(Encoding.UTF8.GetBytes(message));
            }

            return syslogMsg.ToArray();
        }

        //http://www.syslog.org/logged/logging-and-syslog-best-practices/
        public static void SendMessage(Level priority, object originator, string theMessage)
        {
            var processId = "";
            if (originator is string)
                processId = originator as string;
            else
                processId = originator.GetType().ToString();
            var syslogServer = ConfigurationManager.AppSettings["SyslogServer"];
            if (string.IsNullOrWhiteSpace(syslogServer))
                syslogServer = "127.0.0.1";

            using (var udp = new UdpClient(syslogServer, 514))
            {
                // Create a byte to hold our strParams (data) in           
                var rawMsg = ConstructMessage(ProcessName, priority, Facility, "", theMessage, processId);
                udp.Client.SendBufferSize = 4096;
                udp.Send(rawMsg, rawMsg.Length);
                udp.Close();
            }
        }

        public static void SendInformation(object originator, string infoMessage)
        {
            SendMessage(Level.Informational, originator, infoMessage);
        }

        public static void SendError(object originator, string errorMessage)
        {
            SendMessage(Level.Error, originator, errorMessage);
        }

        public static void SendWarning(object originator, string infoMessage)
        {
            SendMessage(Level.Warning, originator, infoMessage);
        }

        public static void SendError(object originator, Exception e)
        {
            var errorMessage = e.Message;
            //if (e.StackTrace != null)
            //    errorMessage += " stack:" + e.StackTrace;
            SendMessage(Level.Error, originator, errorMessage);

            if (e.InnerException != null) //recurse down the inner exceptions
                SendError(originator, e.InnerException);
        }

        public static void SendCriticalError(object originator, Exception e)
        {
            var errorMessage = e.Message;
            //if (e.StackTrace != null)
            //    errorMessage += " stack:" + e.StackTrace;
            SendMessage(Level.Alert, originator, errorMessage);

            if (e.InnerException != null) //recurse down the inner exceptions
                SendError(originator, e.InnerException);
        }

        public static void SendCriticalError(object originator, string errorMessage)
        {
            SendMessage(Level.Alert, originator, errorMessage);
        }
    }
}