using System.Collections.Generic;
using MediatR;
using ServiceBusDriver.Core.Constants;

namespace ServiceBusDriver.Shared.Features.Message
{
    public class SearchRequest : IRequest<List<MessageResponseDto>>
    {
        public string InstanceId { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
        public bool SearchDeadLetter { get; set; }
        public string KeyPath { get; set; }
        public string Value { get; set; }
        public string MatchType { get; set; }
        public string ContentType { get; set; }
        public int PrefetchCount { get; set; } = MessageConstants.DefaultPrefetchCount;
        public int MaxMessages { get; set; } = MessageConstants.DefaultMaxMessages;
    }
}