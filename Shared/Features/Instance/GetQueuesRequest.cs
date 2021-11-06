using System.Collections.Generic;
using MediatR;
using ServiceBusDriver.Shared.Features.Queue;
using ServiceBusDriver.Shared.Features.Topic;

namespace ServiceBusDriver.Shared.Features.Instance
{
    public class GetQueuesRequest : IRequest<List<QueueResponseDto>>
    {
        public GetQueuesRequest(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}