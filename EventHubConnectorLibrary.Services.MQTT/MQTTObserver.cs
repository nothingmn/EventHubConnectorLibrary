using EventHubConnectorLibrary.Contracts;
using EventHubConnectorLibrary.Core;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace EventHubConnectorLibrary.Services.MQTT
{
    public class MQTTObserver : IObserver<EventHubMessage>
    {
        private readonly ILog _log;
        private readonly string _topicFormat;

        public string MessageFilter { get; set; }

        private MqttClient client;

        public MQTTObserver(ILog log, string host, string topicFormat)
        {
            _log = log;
            _topicFormat = topicFormat;
            client = new MqttClient(host);
            string clientId = System.Guid.NewGuid().ToString();
            client.Connect(clientId);
        }

        public void OnNext(EventHubMessage value)
        {
            var body = value.Body;
            var content = System.Text.Encoding.UTF8.GetString(body);
            var topic = string.Format(_topicFormat, value.PartitionKey);
            client.Publish(topic, body);
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