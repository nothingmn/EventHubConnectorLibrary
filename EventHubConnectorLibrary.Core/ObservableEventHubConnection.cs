using System;
using System.Net;
using System.Threading.Tasks;
using EventHubConnectorLibrary.Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace EventHubConnectorLibrary.Core
{
    public class ObservableEventHubConnection : IObservable<EventData>
    {
        private readonly IConfiguration _configuration;
        private readonly ILog _log;

        public ObservableEventHubConnection(IConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;
            var manager = NamespaceManager.CreateFromConnectionString(configuration.EventHubConnectionString);            
            //var desc = manager.CreateEventHubIfNotExistsAsync(configuration.EventHubPath).Result;            
            //var client = Microsoft.ServiceBus.Messaging.EventHubClient.CreateFromConnectionString(configuration.EventHubConnectionString, configuration.EventHubPath);            
            _host = new EventProcessorHost(Dns.GetHostName(), configuration.EventHubPath, configuration.EventHubConsumerGroup, configuration.EventHubConnectionString, configuration.BlobStorageConnectionString, configuration.LeaseContainerName);

            _factory = new EventHubProcessorFactory<EventData>(_configuration);

            _log.Info("Observable Hub Ready...").Wait();

        }

        public async Task Disconnect()
        {
            await _log.Info("Observable Hub disconnecting subscribers...");
            _factory.Disconnect();

            await _log.Info("Observable Hub disconnecting from Event Hub...");
            await _host.UnregisterEventProcessorAsync();

        }

        private EventProcessorHost _host = null;
        public async Task Connect()
        {
            var options = EventProcessorOptions.DefaultOptions;
            options.MaxBatchSize = _configuration.MaxBatchSize;
            options.ReceiveTimeOut = _configuration.ReceiveTimeOut;
            if(_configuration.InitialOffsetProvider!=null) options.InitialOffsetProvider = _configuration.InitialOffsetProvider;
            options.InvokeProcessorAfterReceiveTimeout = _configuration.InvokeProcessorAfterReceiveTimeout;
            options.PrefetchCount = _configuration.PrefetchCount;

            await _log.Info("Observable Hub connecting event processor factory");
            await _host.RegisterEventProcessorFactoryAsync(_factory, options);

            await _log.Info("Observable Hub connected event processor factory");

        }

        private void Options_ExceptionReceived(object sender, ExceptionReceivedEventArgs e)
        {
            _log.Error(e.Exception, "Observable Hub event processor factory failed", sender).Wait();
        }

        private EventHubProcessorFactory<EventData> _factory;

        public IDisposable Subscribe(IObserver<EventData> observer)
        {
            _log.Info("Observable Hub subscribing observer").Wait();

            return _factory.Subscribe(observer);
        }
    }
}