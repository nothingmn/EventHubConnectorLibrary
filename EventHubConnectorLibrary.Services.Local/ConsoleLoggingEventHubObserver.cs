using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHubConnectorLibrary.Contracts;
using Microsoft.ServiceBus.Messaging;

namespace TestConsole
{
    public class ConsoleLoggingEventHubObserver : IObserver<EventData>
    {
        private readonly ILog _log;

        public ConsoleLoggingEventHubObserver(ILog log)
        {
            _log = log;
        }

        public void OnNext(EventData value)
        {
            var msg = System.Text.Encoding.UTF8.GetString(value.GetBytes());
            var props = new Dictionary<string, string>()
            {
                {"Message", msg},
                {"PartitionKey", value.PartitionKey},
                {"Offset", value.Offset},
                {"SequenceNumber", value.SequenceNumber.ToString()},
            };
            if (value.Properties != null)
            {
                foreach (var p in value.Properties)
                {
                    props.Add(p.Key, p.Value.ToString());
                }
            }

            if (value.SystemProperties != null)
            {
                foreach (var p in value.SystemProperties)
                {
                    props.Add(p.Key, p.Value.ToString());
                }
            }


            _log.Event("Message received:", props );
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
            _log.Info("Disconnecting");

        }
    }
}
