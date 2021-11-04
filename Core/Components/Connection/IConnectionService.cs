using System.Threading;
using System.Threading.Tasks;
using ServiceBusDriver.Core.Models.Features.Connection;
using ServiceBusDriver.Core.Models.Features.Instance;

namespace ServiceBusDriver.Core.Components.Connection
{
    public interface IConnectionService
    {
        Task<ServiceBusNamespaceModel> ProcessConnectionString(string connectionString);
        Task<ConnectionSettingsModel> TestConnection(string connectionString, CancellationToken cancellationToken = default);
    }
}