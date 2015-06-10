using EventHubConnectorLibrary.Contracts;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubConnectorLibrary.Core
{
    public class ObservableDeployment : IDeployment
    {
        private readonly IObserver<EventHubMessage> _observer;
        private readonly IConfiguration _configuration;
        private readonly ILog _log;
        private CancellationToken _cancellationToken;
        private ObservableEventHubConnection _hub;

        public ObservableDeployment(IObserver<EventHubMessage> observer, IConfiguration configuration, ILog log, ObservableEventHubConnection hub = null)
        {
            _observer = observer;
            _configuration = configuration;
            _log = log;

            if (hub == null) hub = new ObservableEventHubConnection(_configuration, _log);
            _hub = hub;
        }

        public async Task<bool> Deploy(CancellationToken cancellationToken, string[] args = null)
        {
            _cancellationToken = cancellationToken;

            _hub.Subscribe(_observer);
            if (!_hub.Connected && !_hub.Connecting)
            {
                await _hub.Connect();
            }

            //not awaited so the execution can continue off to other things.
            //The obsever has its own wait loop and monitors our cancel token to know when to dump
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(1000).Wait();
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        if (_hub.Connected && !_hub.Disconnecting)
                        {
                            _hub.Disconnect();
                        }
                        break;
                    }
                }
            }, TaskCreationOptions.LongRunning);
            return true;
        }
    }
}