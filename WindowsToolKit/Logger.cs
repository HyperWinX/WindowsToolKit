using System;
using System.IO;

namespace WindowsToolKit
{
    internal class Logger
    {
        public readonly string logfile = "log.log";
        public void Log(string level, string message)
        {
            string logMessage = $"{DateTime.Now} [{level}] {message}";
            File.AppendAllText(logfile, logMessage + Environment.NewLine);
        }
        public void LogError(string message)
        {
            Log("ERROR", message);
        }
        public void LogWarning(string message)
        {
            Log("WARNING", message);
        }
        public void LogInfo(string message)
        {
            Log("INFO", message);
        }
    }
}
