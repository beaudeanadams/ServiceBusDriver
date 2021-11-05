using Azure.Messaging.ServiceBus.Administration;

namespace ServiceBusDriver.Core.Models.Features.Queue
{
    public class QueueResponse
    {
        public QueueProperties QueueProperties { get; set; }
        public QueueRuntimeProperties RunTimeProperties { get; set; }
    }
}