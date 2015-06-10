using EventHubConnectorLibrary.Contracts;
using EventHubConnectorLibrary.Core;
using Microsoft.ServiceBus.Messaging;
using System;

namespace EventHubConnectorLibrary.Services.MySql
{
    public class MySqlObserver : IObserver<EventHubMessage>
    {
        private readonly string _connectionString;

        public MySqlObserver(ILog log, string connectionString)
        {
            _connectionString = connectionString;
            //client.Connect(clientId);
        }

        public void OnNext(EventHubMessage value)
        {
            var body = value.Body;
            var content = System.Text.Encoding.UTF8.GetString(body);
            //client.Publish(topic, body);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}