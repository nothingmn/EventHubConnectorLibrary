EventHubConnectorLibrary
========================

A generic connector + sample libraries for establishing a connection to an Event Hub on Azure.  Message ingress will be Exposed via an IObservable<EventData>.  

This project also takes care of the overhead of the Worker Role, CheckPointing, Consumer Groups, etc..

It allows you to focus just on your message handling and avoid dealing with the EventHub and worker role complexities.

We also support mulitple deployments, where a deployment is a self-contained ObservableEventHubConnection along with its Observers.  You can have multiple subscribers/observers on the same partition within the consumer group, or have multiple instances across multiple partitions with their own subscribers, etc..

