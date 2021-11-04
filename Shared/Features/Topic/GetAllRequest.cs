using System.Collections.Generic;
using MediatR;
using ServiceBusDriver.Core.Models.Features.Topic;

namespace ServiceBusDriver.Shared.Features.Topic
{
    public class GetAllRequest : IRequest<List<TopicWithInstanceModel>>
    {
    }
}