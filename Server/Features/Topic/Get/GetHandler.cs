using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Topic;
using ServiceBusDriver.Server.Services.Validations;
using ServiceBusDriver.Shared.Features.Topic;

namespace ServiceBusDriver.Server.Features.Topic.Get
{
    public class GetHandler : IRequestHandler<GetRequest, TopicResponseDto>
    {
        private readonly ITopicService _topicService;
        private readonly IMapper _mapper;
        private readonly IDbFetchHelper _dbFetchHelper;
        private readonly ILogger<GetHandler> _logger;

        public GetHandler(ITopicService topicService, ILogger<GetHandler> logger, IMapper mapper, IDbFetchHelper dbFetchHelper)
        {
            _topicService = topicService;
            _logger = logger;
            _mapper = mapper;
            _dbFetchHelper = dbFetchHelper;
        }

        public async Task<TopicResponseDto> Handle(GetRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            await _dbFetchHelper.BelongToCurrentUser(request.InstanceId, cancellationToken);

            var result = await _topicService.GetTopicByName(request.InstanceId, request.TopicName, cancellationToken);

            var response = _mapper.Map<TopicResponseDto>(result);


            _logger.LogTrace("Finish {0}", nameof(Handle));

            return response;
        }
    }
}