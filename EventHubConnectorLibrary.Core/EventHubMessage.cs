using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubConnectorLibrary.Core
{
    public class EventHubMessage
    {
        private readonly EventData _data;

        public string Offset { get { return _data.Offset; } }

        public long SerializedSizeInBytes { get { return _data.SerializedSizeInBytes; } }

        public string PartitionKey { get { return _data.PartitionKey; } }

        public long SequenceNumber { get { return _data.SequenceNumber; } }

        public DateTime EnqueuedTimeUtc { get { return _data.EnqueuedTimeUtc; } }

        public IDictionary<string, object> Properties { get { return _data.Properties; } }

        public IDictionary<string, object> SystemProperties { get { return _data.SystemProperties; } }

        public EventHubMessage(EventData data)
        {
            _data = data;
        }

        private byte[] _body = null;
        private static object _lock = new object();

        private bool read = false;

        public byte[] Body
        {
            get
            {
                if (read) return _body;
                if (_data == null) return null;
                lock (_lock)
                {
                    if (_body != null || read) return _body;
                    _body = _data.GetBytes();
                    read = true;
                }
                return _body;
            }
        }
    }
}