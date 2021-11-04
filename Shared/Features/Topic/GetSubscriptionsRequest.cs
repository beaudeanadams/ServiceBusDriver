using System.Collections.Generic;
using MediatR;
using ServiceBusDriver.Shared.Features.Subscription;

namespace ServiceBusDriver.Shared.Features.Topic
{
    public class GetSubscriptionsRequest : IRequest<List<SubscriptionResponseDto>>
    {
        public string TopicName { get; set; }
        public string InstanceId { get; set; }
    }
}