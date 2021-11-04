using Azure.Messaging.ServiceBus;

namespace ServiceBusDriver.Core.Models.Features.Message
{
    public class MessageResponse
    {
        public string Payload { get; set; }
        public bool DeadLetterQueue { get; set; }
        public ServiceBusReceivedMessage Message { get; set; }
    }
}