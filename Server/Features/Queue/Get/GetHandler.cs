using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Queue;
using ServiceBusDriver.Server.Services.Validations;
using ServiceBusDriver.Shared.Features.Queue;

namespace ServiceBusDriver.Server.Features.Queue.Get
{
    public class GetHandler : IRequestHandler<GetRequest, QueueResponseDto>
    {
        private readonly IQueueService _queueService;
        private readonly IMapper _mapper;
        private readonly IDbFetchHelper _dbFetchHelper;
        private readonly ILogger<GetHandler> _logger;

        public GetHandler(IQueueService queueService, ILogger<GetHandler> logger, IMapper mapper, IDbFetchHelper dbFetchHelper)
        {
            _queueService = queueService;
            _logger = logger;
            _mapper = mapper;
            _dbFetchHelper = dbFetchHelper;
        }

        public async Task<QueueResponseDto> Handle(GetRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            await _dbFetchHelper.BelongToCurrentUser(request.InstanceId, cancellationToken);

            var result = await _queueService.GetQueueByName(request.InstanceId, request.QueueName, cancellationToken);

            var response = _mapper.Map<QueueResponseDto>(result);


            _logger.LogTrace("Finish {0}", nameof(Handle));

            return response;
        }
    }
}