using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHubConnectorLibrary.Contracts;
using Microsoft.ServiceBus.Messaging;

namespace EventHubConnectorLibrary.Core
{
    public class EventProcessor : IEventProcessor, IObservable<EventData>
    {
        private readonly IConfiguration _configuration;
        private readonly ILog _log;

        public EventProcessor(IConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;

            if (_configuration.CheckpointFunc == null)
            {
                _configuration.CheckpointFunc = () => true;  //default is to always checkpoint after each batch.
            }

        }

        private PartitionContext _context;

        public async Task OpenAsync(PartitionContext context)
        {
            await _log.Info("Opened Event Processor: EventHubPath:{0}, Consumer Group:{1}, PartitionID:{2}", context.EventHubPath, context.ConsumerGroupName, context.Lease.PartitionId);
            _context = context;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            await _log.Info("Processing Events: EventHubPath:{0}, Consumer Group:{1}, PartitionID:{2}, Message Count:{3}", context.EventHubPath, context.ConsumerGroupName, context.Lease.PartitionId, messages.LongCount());
            foreach (var m  in messages)
            {
                foreach (var s in _subscribers)
                {
                    s.OnNext(m);
                }
            }

            var performCheckpoint = _configuration.CheckpointFunc();
            if (performCheckpoint)
            {
                await context.CheckpointAsync();
            }
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            await _log.Info("Closing Event Processor: EventHubPath:{0}, Consumer Group:{1}, PartitionID:{2}", context.EventHubPath, context.ConsumerGroupName, context.Lease.PartitionId);
            foreach (var s in _subscribers)
            {
                s.OnCompleted();
            }
        }

        private List<IObserver<EventData>> _subscribers = new List<IObserver<EventData>>();

        public async Task Disconnect()
        {
            await _log.Info("Disconnect Event Processor: EventHubPath:{0}, Consumer Group:{1}, PartitionID:{2}", _context.EventHubPath, _context.ConsumerGroupName, _context.Lease.PartitionId);
            foreach (var s in _subscribers)
            {
                s.OnCompleted();
            }
        }
        public IDisposable Subscribe(IObserver<EventData> observer)
        {
            if (!_subscribers.Contains(observer))
            {
                _subscribers.Add(observer);
            }
            return new Unsubscriber<EventData>(_subscribers, observer);
        }
    }
}