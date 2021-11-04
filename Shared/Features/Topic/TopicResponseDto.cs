using System;

namespace ServiceBusDriver.Shared.Features.Topic
{
    public class TopicResponseDto
    {
        public string Name { get; set; }
        public long MaxSizeInMegabytes { get; set; }
        public string EntityStatus { get; set; }
        public string UserMetadata { get; set; }
        public TopicStats TopicStats { get; set; }
        public TopicDateProperties TopicDateProperties { get; set; }
        public TopicTimeDetails TopicTimeDetails { get; set; }
        public TopicCheckBoxProperties TopicCheckBoxProperties { get; set; }
    }

    public class TopicStats
    {
        public long SizeInBytes { get; set; }
        public int SubscriptionCount { get; set; }
        public long ScheduledMessageCount { get; set; }

    }

    public class TopicDateProperties
    {
        public DateTimeOffset AccessedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

    }

    public class TopicTimeDetails
    {
        public string DefaultMessageTimeToLive { get; set; }
        public string AutoDeleteOnIdle { get; set; }
        public string DuplicateDetectionHistoryTimeWindow { get; set; }
    }

    public class TopicCheckBoxProperties
    {
        public bool EnablePartitioning { get; set; }
        public bool SupportOrdering { get; set; }
        public bool EnableBatchedOperations { get; set; }
        public bool RequiresDuplicateDetection { get; set; }

    }
}
