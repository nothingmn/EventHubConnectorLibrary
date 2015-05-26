using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventHubConnectorLibrary.Core;
using EventHubConnectorLibrary.Services.Local;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            var token = source.Token;

            var deployment = typeof (ConsoleLoggingEventHubObserverDeployment);
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
