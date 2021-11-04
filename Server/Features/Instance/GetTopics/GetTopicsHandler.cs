using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Instance;
using ServiceBusDriver.Core.Components.Topic;
using ServiceBusDriver.Server.Services.Validations;
using ServiceBusDriver.Shared.Features.Instance;
using ServiceBusDriver.Shared.Features.Topic;

namespace ServiceBusDriver.Server.Features.Instance.GetTopics
{
    public class GetTopicsHandler : IRequestHandler<GetTopicsRequest, List<TopicResponseDto>>
    {
        private readonly ITopicService _topicService;
        private readonly IMapper _mapper;
        private readonly IDbFetchHelper _dbFetchHelper;
        private readonly ILogger<GetTopicsHandler> _logger;

        public GetTopicsHandler(ITopicService topicService, IInstanceService instanceService, IMapper mapper, ILogger<GetTopicsHandler> logger, IDbFetchHelper dbFetchHelper)
        {
            _topicService = topicService;
            _mapper = mapper;
            _logger = logger;
            _dbFetchHelper = dbFetchHelper;
        }


        public async Task<List<TopicResponseDto>> Handle(GetTopicsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            var instance = await _dbFetchHelper.FetchIfBelongsToCurrentUser(request.Id, cancellationToken);

            var topics = await _topicService.GetTopicsForInstance(instance.Id, cancellationToken);

            var response = _mapper.Map<List<TopicResponseDto>>(topics);

            _logger.LogTrace("Finish {0}", nameof(Handle));

            return response;
        }
    }
}