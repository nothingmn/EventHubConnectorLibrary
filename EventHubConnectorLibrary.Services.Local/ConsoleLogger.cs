using EventHubConnectorLibrary.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventHubConnectorLibrary.Services.Local
{
    public class ConsoleLogger : ILog
    {
        public async Task Debug(string message, params object[] args)
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(System.DateTime.Now.ToLocalTime() + " DEBUG: " + String.Format(message, args));
            System.Console.ResetColor();
        }

        public async Task Info(string message, params object[] args)
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(System.DateTime.Now.ToLocalTime() + " INFO: " + String.Format(message, args));
            System.Console.ResetColor();
        }

        public async Task Event(string eventName, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null)
        {
            System.Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(System.DateTime.Now.ToLocalTime() + " EVENT: " + eventName);
            if (properties != null)
            {
                foreach (var p in properties)
                {
                    Console.WriteLine(string.Format("\t{0}={1}", p.Key, p.Value));
                }
            }
            System.Console.ResetColor();
        }

        public async Task Error(Exception exception, string message, params object[] args)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(System.DateTime.Now.ToLocalTime() + " ERROR: " + exception);
            System.Console.ResetColor();
        }

        public async Task Fatal(Exception exception, string message, params object[] args)
        {
            System.Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(System.DateTime.Now.ToLocalTime() + " FATAL: " + exception);
            System.Console.ResetColor();
        }
    }
}