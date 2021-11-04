using Azure.Messaging.ServiceBus.Administration;

namespace ServiceBusDriver.Core.Models.Features.Topic
{
    public class TopicResponse
    {
        public TopicProperties TopicProperties { get; set; }
        public TopicRuntimeProperties RunTimeProperties { get; set; }
    }
}