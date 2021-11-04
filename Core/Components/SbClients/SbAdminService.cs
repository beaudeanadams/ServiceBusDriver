using Azure.Core;
using Azure.Messaging.ServiceBus.Administration;

namespace ServiceBusDriver.Core.Components.SbClients
{
    public class SbAdminService : ISbAdminService
    {
        private ServiceBusAdministrationClient _serviceBusAdministrationClient;

        private readonly ServiceBusAdministrationClientOptions _adminOptions = new ServiceBusAdministrationClientOptions
        {
            Retry = {MaxRetries = 1, Mode = RetryMode.Exponential}
        };

        public ServiceBusAdministrationClient Client(string connectionString)
        {
            return _serviceBusAdministrationClient ??= new ServiceBusAdministrationClient(connectionString, _adminOptions);
        }
    }
}