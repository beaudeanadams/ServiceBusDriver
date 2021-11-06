using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceBusDriver.Core.Models.Features.Queue;

namespace ServiceBusDriver.Core.Components.Queue
{
    public interface IQueueService
    {
        Task<List<QueueResponse>> ListQueues(CancellationToken cancellationToken = default);
        Task<List<QueueResponse>> GetQueuesForInstance(string instanceId, CancellationToken cancellationToken = default);
        Task<QueueResponse> GetQueueByName(string instanceId, string name, CancellationToken cancellationToken = default);
    }
}