using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceBusDriver.Core.Models.Features.Subscription;

namespace ServiceBusDriver.Core.Components.Subscription
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionResponse>> GetSubscriptionsForTopic(string instanceId, string topicName, CancellationToken cancellationToken = default);
        Task<SubscriptionResponse> GetSubscriptionByName(string instanceId, string topicName, string subscriptionName, CancellationToken cancellationToken = default);
    }
}