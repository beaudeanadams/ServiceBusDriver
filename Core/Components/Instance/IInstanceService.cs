using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceBusDriver.Core.Models.Features.Instance;

namespace ServiceBusDriver.Core.Components.Instance
{
    public interface IInstanceService
    {
        Task<List<InstanceResponse>> ListInstances(CancellationToken cancellationToken);
        Task<List<ServiceBusInstanceModel>> ListInstancesFull(CancellationToken cancellationToken);
        Task<InstanceResponse> GetInstance(string id, CancellationToken cancellationToken = default);
        Task<ServiceBusInstanceModel> GetInstanceFull(string id, CancellationToken cancellationToken = default);
    }
}