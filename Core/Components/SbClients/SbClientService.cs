using Azure.Messaging.ServiceBus;

namespace ServiceBusDriver.Core.Components.SbClients
{
    public class SbClientService : ISbClientService
    {
        private ServiceBusClient _serviceBusClient;

        private readonly ServiceBusClientOptions _clientOptions = new()
        {
            RetryOptions = new ServiceBusRetryOptions
            {
                MaxRetries = 1,
                Mode = ServiceBusRetryMode.Exponential
            },
            TransportType = ServiceBusTransportType.AmqpTcp,
            EnableCrossEntityTransactions = false
        };

        public ServiceBusClient Client(string connectionString)
        {
            return _serviceBusClient ??= new ServiceBusClient(connectionString, _clientOptions);
        }

        public ServiceBusClient Client(string connectionString, ServiceBusTransportType transportType)
        {
            return _serviceBusClient ??= new ServiceBusClient(connectionString, new ServiceBusClientOptions
            {
                RetryOptions = new ServiceBusRetryOptions
                {
                    MaxRetries = 1,
                    Mode = ServiceBusRetryMode.Exponential
                },
                TransportType = transportType,
                EnableCrossEntityTransactions = false
            });
        }
    }
}