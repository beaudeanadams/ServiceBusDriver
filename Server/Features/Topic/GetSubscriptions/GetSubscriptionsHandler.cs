using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Subscription;
using ServiceBusDriver.Shared.Features.Subscription;
using ServiceBusDriver.Shared.Features.Topic;

namespace ServiceBusDriver.Server.Features.Topic.GetSubscriptions
{
    public class GetSubscriptionsHandler : IRequestHandler<GetSubscriptionsRequest, List<SubscriptionResponseDto>>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSubscriptionsHandler> _logger;

        public GetSubscriptionsHandler(ISubscriptionService subscriptionService, ILogger<GetSubscriptionsHandler> logger, IMapper mapper)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<SubscriptionResponseDto>> Handle(GetSubscriptionsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            var result = await _subscriptionService.GetSubscriptionsForTopic(request.InstanceId, request.TopicName, cancellationToken);

            _logger.LogTrace("Finish {0}", nameof(Handle));

            var response = _mapper.Map<List<SubscriptionResponseDto>>(result);

            return response;
        }
    }
}