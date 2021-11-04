using Azure.Messaging.ServiceBus;

namespace ServiceBusDriver.Core.Components.SbClients
{
    public interface ISbClientService
    {
        ServiceBusClient Client(string connectionString);
        ServiceBusClient Client(string connectionString, ServiceBusTransportType transportType);
    }
}