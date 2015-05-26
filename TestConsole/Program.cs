using EventHubConnectorLibrary.Core;
using EventHubConnectorLibrary.Services.Local;
using System;
using System.Threading;

namespace TestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            var token = source.Token;

            var deployment = typeof(ConsoleLoggingEventHubObserverDeployment);
            var deployed = DeploymentManager.Deploy(deployment, token).Result;

            if (!deployed)
            {
                throw new NotSupportedException(string.Format("Could not deploy:{0}", deployment.FullName));
            }

            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
            }
            source.Cancel();
        }
    }
}