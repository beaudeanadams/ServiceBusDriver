using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Instance;
using ServiceBusDriver.Core.Components.Topic;
using ServiceBusDriver.Core.Models.Features.Instance;
using ServiceBusDriver.Core.Models.Features.Topic;
using ServiceBusDriver.Shared.Features.Topic;

namespace ServiceBusDriver.Server.Features.Topic.GetAll
{
    public class GetAllHandler : IRequestHandler<GetAllRequest, List<TopicWithInstanceModel>>
    {
        private readonly IInstanceService _instanceService;
        private readonly ITopicService _topicService;
        private readonly ILogger<GetAllHandler> _logger;

        public GetAllHandler(ITopicService topicService, IInstanceService instanceService, ILogger<GetAllHandler> logger)
        {
            _instanceService = instanceService;
            _topicService = topicService;
            _logger = logger;
        }

        public async Task<List<TopicWithInstanceModel>> Handle(GetAllRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            var instances = await _instanceService.ListInstancesFull(cancellationToken);

            var response = new List<TopicWithInstanceModel>();

            foreach (var instance in instances)
            {
                var topics = await _topicService.GetTopicsForInstance(instance.Id, cancellationToken);

                response.Add(new TopicWithInstanceModel
                {
                    Topics = topics.OrderBy(x => x.TopicProperties.Name).ToList(),
                    TopicsCount = topics.Count,
                    Instance = new InstanceResponse
                    {
                        Id = instance.Id,
                        Name = instance.Name
                    }
                });
            }

            _logger.LogTrace("Finish {0}", nameof(Handle));

            return response;
        }
    }
}