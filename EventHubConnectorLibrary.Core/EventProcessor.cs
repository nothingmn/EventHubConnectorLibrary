using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHubConnectorLibrary.Contracts;
using Microsoft.ServiceBus.Messaging;

namespace EventHubConnectorLibrary.Core
{
    public class EventProcessor : IEventProcessor, IObservable<EventData>
    {
        private readonly IConfiguration _configuration;

        public EventProcessor(IConfiguration configuration)
        {
            _configuration = configuration;

            if (_configuration.CheckpointFunc == null)
            {
                _configuration.CheckpointFunc = () => true;  //default is to always checkpoint after each batch.
            }

        }

        private PartitionContext _context;

        public async Task OpenAsync(PartitionContext context)
        {
            _context = context;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
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
            foreach (var s in _subscribers)
            {
                s.OnCompleted();
            }
        }

        private List<IObserver<EventData>> _subscribers = new List<IObserver<EventData>>();

        public async Task Disconnect()
        {
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