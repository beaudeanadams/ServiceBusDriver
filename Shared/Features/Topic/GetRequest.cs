using MediatR;

namespace ServiceBusDriver.Shared.Features.Topic
{
    public class GetRequest : IRequest<TopicResponseDto>
    {
        public string TopicName { get; set; }
        public string InstanceId { get; set; }
    }
}