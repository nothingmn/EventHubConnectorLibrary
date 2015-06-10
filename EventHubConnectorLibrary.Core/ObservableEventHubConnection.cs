using EventHubConnectorLibrary.Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Threading.Tasks;

namespace EventHubConnectorLibrary.Core
{
    public class ObservableEventHubConnection : IObservable<EventHubMessage>
    {
        private readonly IConfiguration _configuration;
        private readonly ILog _log;

        public ObservableEventHubConnection(IConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;

            _log.Info("Setting up Event Hub Connectivity...").Wait();
            var manager = NamespaceManager.CreateFromConnectionString(_configuration.EventHubConnectionString);
            _host = new EventProcessorHost(configuration.HostName, _configuration.EventHubPath, _configuration.EventHubConsumerGroup, _configuration.EventHubConnectionString, _configuration.BlobStorageConnectionString, _configuration.LeaseContainerName);
            _factory = new EventHubProcessorFactory<EventHubMessage>(_configuration, _log);
        }

        public bool Disconnecting { get; set; }

        public bool Connecting { get; set; }

        public bool Connected { get; set; }

        public async Task Disconnect()
        {
            Disconnecting = true;
            await _log.Info("Observable Hub disconnecting subscribers...");
            _factory.Disconnect();

            await _log.Info("Observable Hub disconnecting from Event Hub...");
            await _host.UnregisterEventProcessorAsync();
            Connected = false;
        }

        private EventProcessorHost _host = null;

        public async Task Connect()
        {
            Connecting = true;
            var options = EventProcessorOptions.DefaultOptions;
            options.MaxBatchSize = _configuration.MaxBatchSize;
            options.ReceiveTimeOut = _configuration.ReceiveTimeOut;

            if (_configuration.InitialOffsetProvider != null) options.InitialOffsetProvider = _configuration.InitialOffsetProvider;
            options.InvokeProcessorAfterReceiveTimeout = _configuration.InvokeProcessorAfterReceiveTimeout;
            options.PrefetchCount = _configuration.PrefetchCount;
            options.ExceptionReceived += Options_ExceptionReceived;

            await _log.Info("Observable Hub Ready, attaching event processor factory...");

            await _log.Info("Observable Hub connecting event processor factory, this should take about 30 seconds...");
            await _host.RegisterEventProcessorFactoryAsync(_factory, options);

            await _log.Info("Observable Hub connected event processor factory");
            Connected = true;
            Connecting = false;
        }

        private void Options_ExceptionReceived(object sender, ExceptionReceivedEventArgs e)
        {
            _log.Error(e.Exception, "Event Processor had an exception, sender:{0}", sender).Wait();
        }

        private EventHubProcessorFactory<EventHubMessage> _factory;

        public IDisposable Subscribe(IObserver<EventHubMessage> observer)
        {
            _log.Info("Observable Hub subscribing observer").Wait();
            return _factory.Subscribe(observer);
        }
    }
}