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

namespace ServiceBusDriver.Server.Features.Message.GetLast100Messages
{
    public class GetLastNMessagesHandler : IRequestHandler<GetLastNMessages, List<MessageResponseDto>>
    {
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;
        private readonly IDbFetchHelper _dbFetchHelper;
        private readonly ILogger<GetLastNMessagesHandler> _logger;

        public GetLastNMessagesHandler(IMessageService messageService, ILogger<GetLastNMessagesHandler> logger, IMapper mapper, IDbFetchHelper dbFetchHelper)
        {
            _messageService = messageService;
            _logger = logger;
            _mapper = mapper;
            _dbFetchHelper = dbFetchHelper;
        }

        public async Task<List<MessageResponseDto>> Handle(GetLastNMessages request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            await _dbFetchHelper.BelongToCurrentUser(request.InstanceId, cancellationToken);

            var messages = await _messageService.FetchMessages(new FetchMessagesCommand
            {
                InstanceId = request.InstanceId,
                QueueName = request.QueueName,
                TopicName = request.TopicName,
                SubscriptionName = request.SubscriptionName,
                FetchAll = true,
                DeadLetterQueue = request.DeadLetterQueue,
                OrderByDescending = true,
                PrefetchCount = request.PrefetchCount,
                MaxMessages = request.MaxMessages
            }, cancellationToken);

            _logger.LogTrace("Finish {0}", nameof(Handle));

            var totalCount = messages.Count < request.Limit ? messages.Count : request.Limit;

            messages.RemoveRange(totalCount, messages.Count - totalCount);

            var response = _mapper.Map<List<MessageResponseDto>>(messages);

            _logger.LogTrace("Finish {0}", nameof(Handle));

            return response;
        }
    }
}