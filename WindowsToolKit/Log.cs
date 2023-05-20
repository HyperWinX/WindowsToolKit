using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsToolKit
{
    public class Log
    {
        internal static void Success(string text)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[SUCCESS]: " + text);
            Console.ForegroundColor = oldColor;
        }
        internal static void Warning(string text)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[WARNING]: " + text);
            Console.ForegroundColor = oldColor;
        }
        internal static void Error(string text)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR]: " + text);
            Console.ForegroundColor = oldColor;
        }
    }

}
