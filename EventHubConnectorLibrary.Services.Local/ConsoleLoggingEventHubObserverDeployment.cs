using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventHubConnectorLibrary.Contracts;
using EventHubConnectorLibrary.Core;
using TestConsole;

namespace EventHubConnectorLibrary.Services.Local
{
    public class ConsoleLoggingEventHubObserverDeployment : IDeployment
    {
        private CancellationToken _cancellationToken;

        public async Task<bool> Deploy(CancellationToken cancellationToken)
        {

            _cancellationToken = cancellationToken;
            var eventObserver = new ConsoleLoggingEventHubObserver(new ConsoleLogger());
            var hub = new ObservableEventHubConnection(new AppConfigConfiguration(), new ConsoleLogger());
            hub.Subscribe(eventObserver);
            hub.Connect();


            //not awaited so the execution can continue off to other things.  
            //The obsever has its own wait loop and monitors our cancel token to know when to dump
            Task.Factory.StartNew(() =>
            {         
                while (true)
                {
                    Task.Delay(1000).Wait();
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        hub.Disconnect();
                        break;
                    }
                }

            }, TaskCreationOptions.LongRunning);
            return true;
        }
    }
}