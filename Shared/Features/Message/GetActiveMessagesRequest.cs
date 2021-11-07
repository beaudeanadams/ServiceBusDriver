﻿using System.Collections.Generic;
using MediatR;
using ServiceBusDriver.Core.Constants;

namespace ServiceBusDriver.Shared.Features.Message
{
    public class GetActiveMessagesRequest : IRequest<List<MessageResponseDto>>
    {
        public string InstanceId { get; set; }
        public string QueueName { get; set; }
        public string SubscriptionName { get; set; }
        public string TopicName { get; set; }
        public int PrefetchCount { get; set; } = MessageConstants.DefaultPrefetchCount;
        public int MaxMessages { get; set; } = MessageConstants.DefaultMaxMessages; 
        public bool ReceiveAndDelete { get; set; } // Defaulted to false
    }
}