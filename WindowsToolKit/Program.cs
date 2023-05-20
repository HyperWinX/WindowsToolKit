using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

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
        public static StreamWriter logger;

        static void Main(string[] args)
        {
            if (!File.Exists("log.log"))
                File.Create("log.log").Close();
            else
            {
                File.Delete("log.log");
                File.Create("log.log").Close();
            }
            logger = new StreamWriter(File.Open("log.log", FileMode.Open));
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
                    logger.WriteLine($"Request entered: \"{request}\"");
                    QueryHandler.Handle(request[0]);
                }
            }
        }
    }
}