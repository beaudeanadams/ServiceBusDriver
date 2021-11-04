using System.Collections.Generic;
using MediatR;
using ServiceBusDriver.Core.Constants;
using ServiceBusDriver.Shared.Features.Message;

namespace ServiceBusDriver.Shared.Features.Subscription
{
    public class GetLastNMessages : IRequest<List<MessageResponseDto>>
    {
        public string InstanceId { get; set; }
        public string SubscriptionName { get; set; }
        public string TopicName { get; set; }
        public int Limit { get; set; }
        public bool DeadLetterQueue { get; set; }
        public int PrefetchCount { get; set; } = MessageConstants.DefaultPrefetchCount;
        public int MaxMessages { get; set; } = MessageConstants.DefaultMaxMessages;
    }
}