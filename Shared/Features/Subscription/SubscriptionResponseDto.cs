using System;
using System.Collections.Generic;

namespace ServiceBusDriver.Shared.Features.Subscription
{
    public class SubscriptionResponseDto
    {
        public string SubscriptionName { get; set; }
        public string TopicName { get; set; }
        public int MaxDeliveryCount { get; set; }
        public string EntityStatus { get; set; }
        public string ForwardTo { get; set; }
        public string ForwardDeadLetteredMessagesTo { get; set; }
        public string UserMetadata { get; set; }
        public SubscriptionStats SubscriptionStats { get; set; }
        public SubscriptionDateProperties SubscriptionDateProperties { get; set; }
        public SubscriptionTimeDetails SubscriptionTimeDetails { get; set; }
        public SubscriptionCheckBoxProperties SubscriptionCheckBoxProperties { get; set; }

        public List<SubscriptionRule> Rules { get; set; }
    }

    public class SubscriptionRule
    {
        public string Name { get; set; }
        public string Action { get; set; }
        public string RuleFilter { get; set; }
       
    }

    public class SubscriptionStats
    {
        public long TotalMessageCount { get; set; }
        public long ActiveMessageCount { get; set; }
        public long DeadLetterMessageCount { get; set; }
        public long TransferMessageCount { get; set; }
        public long TransferDeadLetterMessageCount { get; set; }
    }

    public class SubscriptionDateProperties
    {
        public DateTimeOffset AccessedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class SubscriptionTimeDetails
    {
        public string LockDuration { get; set; }
        public string DefaultMessageTimeToLive { get; set; }
        public string AutoDeleteOnIdle { get; set; }
    }

    public class SubscriptionCheckBoxProperties
    {
        public bool RequiresSession { get; set; }
        public bool DeadLetteringOnMessageExpiration { get; set; }
        public bool EnableDeadLetteringOnFilterEvaluationExceptions { get; set; }
        public bool EnableBatchedOperations { get; set; }
    }
}