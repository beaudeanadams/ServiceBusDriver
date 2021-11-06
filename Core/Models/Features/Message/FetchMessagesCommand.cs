namespace ServiceBusDriver.Core.Models.Features.Message
{
    public class FetchMessagesCommand
    {
        public string InstanceId { get; set; }
        public string QueueName { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
        public bool FetchAll { get; set; }
        public bool DeadLetterQueue { get; set; }
        public bool OrderByDescending { get; set; }
        public int PrefetchCount { get; set; }
        public int MaxMessages { get; set; }
    }
}