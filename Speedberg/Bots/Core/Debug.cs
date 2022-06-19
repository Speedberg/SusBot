using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;

namespace Speedberg.Bots.Core
{
    public static class Debug
    {
        private static List<string> logs = new List<string>();
        public static int ErrorCount { get; private set; }

        private static void log(string log)
        {
            log = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} {log}";
            Debug.logs.Add(log);

            Console.WriteLine(log);            
        }

        public static void Silent(string log, [CallerFilePath] string file = "")
        {
            string callerClassName = Path.GetFileNameWithoutExtension(file);

            log = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} [SILENT] {log}";

            Debug.logs.Add(log);
        }

        public static void Log(string log, [CallerFilePath] string file = "")
        {
            string callerClassName = Path.GetFileNameWithoutExtension(file);

            Debug.log($"[LOG][{callerClassName}] {log}");
        }

        public static void Warn(string log, [CallerFilePath] string file = "")
        {
            string callerClassName = Path.GetFileNameWithoutExtension(file);

            Console.BackgroundColor = ConsoleColor.Yellow;
            Debug.log($"[WARN][{callerClassName}] {log}");
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void Error(string log, [CallerFilePath] string file = "")
        {
            string callerClassName = Path.GetFileNameWithoutExtension(file);
            ErrorCount += 1;

            Console.BackgroundColor = ConsoleColor.DarkRed;
            Debug.log($"[ERROR][{callerClassName}] {log}");
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void Fatal(string log, [CallerFilePath] string file = "")
        {
            string callerClassName = Path.GetFileNameWithoutExtension(file);
            ErrorCount += 1;

            Console.BackgroundColor = ConsoleColor.Red;
            Debug.log($"[FATAL][{callerClassName}] {log}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}