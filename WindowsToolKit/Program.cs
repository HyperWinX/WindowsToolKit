using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WindowsToolKit
{
    internal class Program
    {
        internal static string version = "1.2.1";
        internal static string build = "prerelease";
        internal static string requestStr;
        internal static List<string> request;
        internal static ProcessStartInfo com;
        internal static string currentPath = @"C:\";
        internal static string currentMode = "WTK";
        internal static string currentTask = null;
        public static bool helpExited = true;
        internal static Logger log = new Logger();

        static void Main(string[] args)
        {
            log.LogInfo("WTK initialized");
            Console.WriteLine($"WindowsToolKit [Version {version}, Build {build}]");
            Console.WriteLine("HyperCorp copyright.");
            while (true)
            {
                if (!helpExited)
                {
                    helpExited = true;
                }
                else
                {
                    Console.Write($"({currentMode}) >>> ");
                    requestStr = Console.ReadLine();
                    request = requestStr.Split(' ').ToList();
                    log.LogInfo($"Catched string {requestStr}, element count {request.Count}");
                    QueryHandler.Handle(request[0]);
                }
            }
        }
    }
}