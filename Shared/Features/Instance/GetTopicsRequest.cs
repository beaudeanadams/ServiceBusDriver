using System.Collections.Generic;
using MediatR;
using ServiceBusDriver.Shared.Features.Topic;

namespace ServiceBusDriver.Shared.Features.Instance
{
    public class GetTopicsRequest : IRequest<List<TopicResponseDto>>
    {
        public GetTopicsRequest(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}