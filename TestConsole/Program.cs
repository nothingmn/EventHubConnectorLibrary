using EventHubConnectorLibrary.Contracts;
using EventHubConnectorLibrary.Core;
using EventHubConnectorLibrary.Services.MQTT;
using EventHubConnectorLibrary.Services.MySql;
using Microsoft.ServiceBus.Messaging;
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

            var config = new AppConfigConfiguration();
            var log = new ConsoleLogger();
            var observableHub = new ObservableEventHubConnection(config, log);

            Deploy(token, args, new ConsoleLoggingEventHubObserver(log), config, log, observableHub);
            Deploy(token, args, new MQTTObserver(log, "localhost", "{0}"), config, log, observableHub);
            Deploy(token, args, new MySqlObserver(log, ""), config, log, observableHub);

            WaitForExit();
            source.Cancel();
        }

        private static void Deploy(CancellationToken token, string[] args, IObserver<EventHubMessage> observer, AppConfigConfiguration config, ILog log, ObservableEventHubConnection observableHub)
        {
            var hub = new ObservableDeployment(observer, config, log, observableHub);
            hub.Deploy(token, args);
        }

        private static void WaitForExit()
        {
            while (Console.ReadKey().Key != ConsoleKey.Q) { }
        }
    }
}