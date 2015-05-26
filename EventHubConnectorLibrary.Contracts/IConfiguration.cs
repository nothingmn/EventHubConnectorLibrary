using System;

namespace EventHubConnectorLibrary.Contracts
{
    public interface IConfiguration
    {
        string EventHubConnectionString { get; }

        string HostName { get; }

        string EventHubPath { get; }

        string EventHubConsumerGroup { get; }

        string BlobStorageConnectionString { get; }

        string LeaseContainerName { get; }

        int MaxBatchSize { get; }

        TimeSpan ReceiveTimeOut { get; }

        int PrefetchCount { get; }

        Func<string, object> InitialOffsetProvider { get; set; }

        bool InvokeProcessorAfterReceiveTimeout { get; }

        Func<bool> CheckpointFunc { get; set; }
    }
}