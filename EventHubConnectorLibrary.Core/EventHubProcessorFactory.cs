using System;
using System.Threading.Tasks;
using EventHubConnectorLibrary.Contracts;
using Microsoft.ServiceBus.Messaging;

namespace EventHubConnectorLibrary.Core
{
    public class EventHubProcessorFactory<T> : IEventProcessorFactory, IObservable<EventData>
    {
        private readonly IConfiguration _configuration;

        public EventHubProcessorFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            processor = new EventProcessor(_configuration);

        }

        private EventProcessor processor = null;

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            processor.OpenAsync(context).Wait();
            return processor;
        }

        public async Task Disconnect()
        {
            processor.Disconnect();
        }

        public IDisposable Subscribe(IObserver<EventData> observer)
        {
            return processor.Subscribe(observer);
        }
    }
}