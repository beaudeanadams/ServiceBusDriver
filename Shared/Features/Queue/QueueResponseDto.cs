using System;

namespace ServiceBusDriver.Shared.Features.Queue
{
    public class QueueResponseDto
    {
        public string Name { get; set; }
        public long MaxSizeInMegabytes { get; set; }
        public string EntityStatus { get; set; }
        public string UserMetadata { get; set; }
        public int MaxDeliveryCount { get; set; }
        public string ForwardTo { get; set; }
        public string ForwardDeadLetteredMessagesTo { get; set; }
        public QueueStats QueueStats { get; set; }
        public QueueDateProperties QueueDateProperties { get; set; }
        public QueueTimeDetails QueueTimeDetails { get; set; }
        public QueueCheckBoxProperties QueueCheckBoxProperties { get; set; }
    }

    public class QueueStats
    {
        public long TotalMessageCount { get; set; }
        public long ActiveMessageCount { get; set; }
        public long DeadLetterMessageCount { get; set; }
        public long TransferMessageCount { get; set; }
        public long TransferDeadLetterMessageCount { get; set; }
        public long SizeInBytes { get; set; }
        public long ScheduledMessageCount { get; set; }
    }


    public class QueueDateProperties
    {
        public DateTimeOffset AccessedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class QueueTimeDetails
    {
        public string DefaultMessageTimeToLive { get; set; }
        public string AutoDeleteOnIdle { get; set; }
        public string DuplicateDetectionHistoryTimeWindow { get; set; }
        public string LockDuration { get; set; }
    }

    public class QueueCheckBoxProperties
    {
        public bool EnablePartitioning { get; set; }
        public bool SupportOrdering { get; set; }
        public bool EnableBatchedOperations { get; set; }
        public bool RequiresDuplicateDetection { get; set; }
        public bool RequiresSession { get; set; }
        public bool DeadLetteringOnMessageExpiration { get; set; }
        public bool EnableDeadLetteringOnFilterEvaluationExceptions { get; set; }
    }
}