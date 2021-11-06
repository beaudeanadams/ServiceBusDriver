using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Message;
using ServiceBusDriver.Core.Models.Features.Message;
using ServiceBusDriver.Server.Services.Validations;
using ServiceBusDriver.Shared.Features.Message;
using ServiceBusDriver.Shared.Features.Subscription;

namespace ServiceBusDriver.Server.Features.Message.GetDeadLetteredMessages
{
    public class GetDeadLetteredMessagesHandler : IRequestHandler<GetDeadLetteredMessagesRequest, List<MessageResponseDto>>
    {
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;
        private readonly IDbFetchHelper _dbFetchHelper;
        private readonly ILogger<GetDeadLetteredMessagesHandler> _logger;

        public GetDeadLetteredMessagesHandler(IMessageService messageService, ILogger<GetDeadLetteredMessagesHandler> logger, IMapper mapper, IDbFetchHelper dbFetchHelper)
        {
            _messageService = messageService;
            _logger = logger;
            _mapper = mapper;
            _dbFetchHelper = dbFetchHelper;
        }

        public async Task<List<MessageResponseDto>> Handle(GetDeadLetteredMessagesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            await _dbFetchHelper.BelongToCurrentUser(request.InstanceId, cancellationToken);

            var result = await _messageService.FetchMessages(new FetchMessagesCommand
            {
                InstanceId = request.InstanceId,
                QueueName = request.QueueName,
                TopicName = request.TopicName,
                SubscriptionName = request.SubscriptionName,
                FetchAll = false,
                DeadLetterQueue = true,
                OrderByDescending = false,
                PrefetchCount = request.PrefetchCount,
                MaxMessages = request.MaxMessages
            }, cancellationToken);

            var response = _mapper.Map<List<MessageResponseDto>>(result);

            _logger.LogTrace("Finish {0}", nameof(Handle));

            return response;
        }
    }
}