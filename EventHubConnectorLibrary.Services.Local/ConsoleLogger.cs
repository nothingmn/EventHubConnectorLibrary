using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHubConnectorLibrary.Contracts;

namespace EventHubConnectorLibrary.Services.Local
{
    public class ConsoleLogger : ILog
    {
        public async Task Debug(string message, params object[] args)
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("DEBUG: " + String.Format(message, args));
            System.Console.ResetColor();

        }

        public async Task Info(string message, params object[] args)
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("INFO: " + String.Format(message, args));
            System.Console.ResetColor();
        }

        public async Task Event(string eventName, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null)
        {
            System.Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("EVENT: " + eventName);
            if (properties != null)
            {
                foreach (var p in properties)
                {
                    Console.WriteLine(string.Format("{0}={1}", p.Key, p.Value));
                }
            }
            System.Console.ResetColor();
        }

        public async Task Error(Exception exception, string message, params object[] args)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + exception);
            System.Console.ResetColor();
        }

        public async Task Fatal(Exception exception, string message, params object[] args)
        {
            System.Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("FATAL: " + exception);
            System.Console.ResetColor();

        }
    }
}
