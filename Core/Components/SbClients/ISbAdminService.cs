using Azure.Messaging.ServiceBus.Administration;

namespace ServiceBusDriver.Core.Components.SbClients
{
    public interface ISbAdminService
    {
        ServiceBusAdministrationClient Client(string connectionString);
    }
}