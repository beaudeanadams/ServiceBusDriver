using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Queue;
using ServiceBusDriver.Server.Services.Validations;
using ServiceBusDriver.Shared.Features.Instance;
using ServiceBusDriver.Shared.Features.Queue;

namespace ServiceBusDriver.Server.Features.Instance.GetQueues
{
    public class GetQueuesHandler : IRequestHandler<GetQueuesRequest, List<QueueResponseDto>>
    {
        private readonly IQueueService _queueService;
        private readonly IMapper _mapper;
        private readonly IDbFetchHelper _dbFetchHelper;
        private readonly ILogger<GetQueuesHandler> _logger;

        public GetQueuesHandler(IQueueService queueService, IMapper mapper, ILogger<GetQueuesHandler> logger, IDbFetchHelper dbFetchHelper)
        {
            _queueService = queueService;
            _mapper = mapper;
            _logger = logger;
            _dbFetchHelper = dbFetchHelper;
        }


        public async Task<List<QueueResponseDto>> Handle(GetQueuesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            var instance = await _dbFetchHelper.FetchIfBelongsToCurrentUser(request.Id, cancellationToken);
            var topics = await _queueService.GetQueuesForInstance(instance.Id, cancellationToken);

            var response = _mapper.Map<List<QueueResponseDto>>(topics);

            _logger.LogTrace("Finish {0}", nameof(Handle));

            return response;
        }
    }
}