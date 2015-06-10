﻿using EventHubConnectorLibrary.Contracts;
using EventHubConnectorLibrary.Core;
using EventHubConnectorLibrary.Services.MQTT;
using EventHubConnectorLibrary.Services.MySql;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Runtime.Remoting.Messaging;
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
            //var observableHub = new ObservableEventHubConnection(config, log);

            //Deploy(token, args, new ConsoleLoggingEventHubObserver(log), config, log, observableHub);
            //Deploy(token, args, new MQTTObserver(log, "localhost", "{0}"), config, log, observableHub);

            var connectionString = "Server=192.168.1.69;Database=telematics;Uid=telematics;Pwd=mojiorocks007;";
            var mySqlObserver = new MySqlObserver(log, connectionString);
            mySqlObserver.SqlCommandAction = message =>
            {
                if (message.Body == null) return null;
                var body = System.Text.Encoding.UTF8.GetString(message.Body);
                var payload = JsonConvert.DeserializeObject(body);

                return "insert into mytable (id, speed) VALUES('{id}', '{speed}')";
            };
            //Deploy(token, args, mySqlObserver, config, log, observableHub);

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