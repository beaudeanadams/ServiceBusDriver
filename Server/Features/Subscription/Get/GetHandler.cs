using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Subscription;
using ServiceBusDriver.Server.Services.Validations;
using ServiceBusDriver.Shared.Features.Subscription;

namespace ServiceBusDriver.Server.Features.Subscription.Get
{
    public class GetHandler : IRequestHandler<GetRequest, SubscriptionResponseDto>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IDbFetchHelper _dbFetchHelper;
        private readonly IMapper _mapper;
        private readonly ILogger<GetHandler> _logger;

        public GetHandler(ISubscriptionService subscriptionService, ILogger<GetHandler> logger, IDbFetchHelper dbFetchHelper, IMapper mapper)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
            _dbFetchHelper = dbFetchHelper;
            _mapper = mapper;
        }

        public async Task<SubscriptionResponseDto> Handle(GetRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            await _dbFetchHelper.BelongToCurrentUser(request.InstanceId, cancellationToken);

            var result = await _subscriptionService.GetSubscriptionByName(request.InstanceId, request.TopicName, request.SubscriptionName, cancellationToken);

            _logger.LogTrace("Finish {0}", nameof(Handle));

            var response = _mapper.Map<SubscriptionResponseDto>(result);

            return response;
        }
    }
}