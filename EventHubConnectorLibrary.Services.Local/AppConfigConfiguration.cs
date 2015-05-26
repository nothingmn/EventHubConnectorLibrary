using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHubConnectorLibrary.Contracts;
using Microsoft.ServiceBus.Messaging;

namespace EventHubConnectorLibrary.Services.Local
{
    public class AppConfigConfiguration : IConfiguration
    {
        private NameValueCollection appSettings = null;
        EventProcessorOptions _defaultOptions = EventProcessorOptions.DefaultOptions;
        public AppConfigConfiguration()
        {
            appSettings = System.Configuration.ConfigurationManager.AppSettings;
        }

        public string EventHubConnectionString
        {
            get
            {
                var c = appSettings["EventHubConnectionString"];
                if (!string.IsNullOrEmpty(c)) return c;
                return appSettings["Microsoft.ServiceBus.ConnectionString"];
            }
        }

        public string EventHubPath
        {
            get { return appSettings["EventHubPath"]; }
        }

        public string EventHubConsumerGroup
        {
            get { return appSettings["EventHubConsumerGroup"]; }
        }

        public string BlobStorageConnectionString
        {
            get { return appSettings["BlobStorageConnectionString"]; }
        }

        public string LeaseContainerName
        {
            get { return appSettings["LeaseContainerName"]; }
        }

        public int MaxBatchSize
        {
            get
            {
                var c = appSettings["MaxBatchSize"];
                if (!string.IsNullOrEmpty(c)) return Int32.Parse(c);
                return _defaultOptions.MaxBatchSize;
            }
        }
        public TimeSpan ReceiveTimeOut
        {
            get
            {
                var c = appSettings["EventHubConnectionString"];
                if (!string.IsNullOrEmpty(c)) return new TimeSpan(0,0,Int32.Parse(c));
                return _defaultOptions.ReceiveTimeOut;
            }
        }
        public int PrefetchCount
        {
            get
            {
                var c = appSettings["EventHubConnectionString"];
                if (!string.IsNullOrEmpty(c)) return int.Parse(c);
                return _defaultOptions.PrefetchCount;
            }
        }
        public Func<string, object> InitialOffsetProvider { get; set; }
        public bool InvokeProcessorAfterReceiveTimeout
        {
            get
            {
                var c = appSettings["EventHubConnectionString"];
                if (!string.IsNullOrEmpty(c)) return bool.Parse(c);
                return _defaultOptions.InvokeProcessorAfterReceiveTimeout;
            }
        }

        public Func<bool> CheckpointFunc { get; set; }
    }
}
