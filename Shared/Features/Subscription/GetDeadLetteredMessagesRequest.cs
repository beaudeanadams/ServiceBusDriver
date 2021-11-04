using System.Collections.Generic;
using MediatR;
using ServiceBusDriver.Core.Constants;
using ServiceBusDriver.Shared.Features.Message;

namespace ServiceBusDriver.Shared.Features.Subscription
{
    public class GetDeadLetteredMessagesRequest : IRequest<List<MessageResponseDto>>
    {
        public string SubscriptionName { get; set; }
        public string TopicName { get; set; }
        public int PrefetchCount { get; set; } = MessageConstants.DefaultPrefetchCount;
        public int MaxMessages { get; set; } = MessageConstants.DefaultMaxMessages;
        public string InstanceId { get; set; }
    }
}