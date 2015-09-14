using EventHubConnectorLibrary.Contracts;
using EventHubConnectorLibrary.Core;
using System;
using System.Collections.Generic;

namespace EventHubConnectorLibrary.Services.Local
{
    public class ConsoleLoggingEventHubObserver : IObserver<EventHubMessage>, IFilter
    {
        private readonly ILog _log;

        public ConsoleLoggingEventHubObserver(ILog log)
        {
            _log = log;
        }

        private static object _lock = new object();

        public void OnNext(EventHubMessage value)
        {
            lock (_lock)
            {
                var msg = System.Text.Encoding.UTF8.GetString(value.Body);

                if (Filter != null && Filter.Length > 0)
                {
                    foreach (var f in Filter)
                    {
                        //YOU SHALL NOT PASS
                        if (!msg.Contains(f)) return;
                    }
                }

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
                        if (!props.ContainsKey(p.Key)) props.Add(p.Key, p.Value.ToString());
                    }
                }

                if (value.SystemProperties != null)
                {
                    foreach (var p in value.SystemProperties)
                    {
                        if (!props.ContainsKey(p.Key)) props.Add(p.Key, p.Value.ToString());
                    }
                }

                _log.Event("Message received:", props);
            }
        }

        public void OnError(Exception error)
        {
            _log.Error(error, "Observed an error");
        }

        public void OnCompleted()
        {
            _log.Info("Disconnecting");
        }

        public string[] Filter { get; set; }
    }
}