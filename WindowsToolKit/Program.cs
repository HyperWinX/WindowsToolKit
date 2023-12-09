using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WindowsToolKit
{
    internal class Program
    {
        internal static string version = "1.2.3";
        internal static string build = "prerelease";
        internal static string requestStr;
        internal static List<string> request;
        internal static ProcessStartInfo com;
        internal static string currentPath;
        internal static string currentMode;
        internal static string currentTask = null;
        public static bool helpExited = true;
        internal static Logger log = new Logger();

        static void Main(string[] args)
        {
            currentPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            currentMode = "WTK";
            log.LogClear();
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
                    GC.Collect();
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