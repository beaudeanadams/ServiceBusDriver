using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceBusDriver.Core.Models.Features.Topic;

namespace ServiceBusDriver.Core.Components.Topic
{
    public interface ITopicService
    {
        Task<List<TopicResponse>> ListTopics(CancellationToken cancellationToken = default);
        Task<List<TopicResponse>> GetTopicsForInstance(string instanceId, CancellationToken cancellationToken = default);
        Task<TopicResponse> GetTopicByName(string instanceId, string name, CancellationToken cancellationToken = default);
    }
}