using System;
using System.Collections.Generic;

namespace ServiceBusDriver.Shared.Features.Message
{
    public class MessageResponseDto
    {
        public string MessageId { get; set; }
        public string Subject { get; set; }
        public long SequenceNumber { get; set; }
        public int DeliveryCount { get; set; }
        public string Payload { get; set; }
        public string ContentType { get; set; }
        public DateTimeOffset EnqueuedTime { get; set; }
        public string TimeToLive { get; set; }
        public string CorrelationId { get; set; }
        public long EnqueuedSequenceNumber { get; set; }
        public bool MessageFromDeadLetterQueue { get; set; }
        public IReadOnlyDictionary<string,string> ApplicationProperties { get; set; }
        public MessageSecondaryProperties MessageSecondaryProperties { get; set; }
        public MessageTimeProperties MessageTimeProperties { get; set; }
        public DeadLetterProperties DeadLetterProperties { get; set; }

    }
    public class MessageSecondaryProperties
    {

        public string To { get; set; }
        public string ReplyTo { get; set; }
        public string PartitionKey { get; set; }
        public string TransactionPartitionKey { get; set; }
        public string SessionId { get; set; }
        public string ReplyToSessionId { get; set; }
        public string LockToken { get; set; }
    }

    public class MessageTimeProperties
    {
        public DateTimeOffset ScheduledEnqueueTime { get; set; }
        public DateTimeOffset LockedUntil { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }

    public class DeadLetterProperties
    {
        public string DeadLetterSource { get; set; }
        public string DeadLetterReason { get; set; }
        public string DeadLetterErrorDescription { get; set; }
    }
}
