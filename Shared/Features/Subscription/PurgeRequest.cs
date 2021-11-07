using MediatR;
using ServiceBusDriver.Shared.Features.Message;
using System.Collections.Generic;

namespace ServiceBusDriver.Shared.Features.Subscription
{
    public class PurgeRequest : IRequest<List<MessageResponseDto>>
    {
        public string InstanceId { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
        public bool IsDeadLetterQueue { get; set; }
    }
}