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
    public class MQTTObserver : IObserver<EventHubMessage>, IFilter
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
            client.ConnectionClosed += Client_ConnectionClosed;
            string clientId = System.Guid.NewGuid().ToString();
            client.Connect(clientId);
        }

        private void Client_ConnectionClosed(object sender, EventArgs e)
        {
            this.OnError(new Exception(sender.ToString()));
        }

        public void OnNext(EventHubMessage value)
        {
            var body = value.Body;
            var content = System.Text.Encoding.UTF8.GetString(body);

            if (Filter != null && Filter.Length > 0)
            {
                foreach (var f in Filter)
                {
                    //YOU SHALL NOT PASS
                    if (!content.Contains(f)) return;
                }
            }

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

        public string[] Filter { get; set; }
    }
}