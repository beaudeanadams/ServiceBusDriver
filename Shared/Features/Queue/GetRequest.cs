using MediatR;

namespace ServiceBusDriver.Shared.Features.Queue
{
    public class GetRequest : IRequest<QueueResponseDto>
    {
        public string QueueName { get; set; }
        public string InstanceId { get; set; }
    }
}