using MediatR;

namespace ServiceBusDriver.Shared.Features.Subscription
{
    public class GetRequest : IRequest<SubscriptionResponseDto>
    {
        public string SubscriptionName { get; set; }
        public string TopicName { get; set; }
        public string InstanceId { get; set; }
    }
}