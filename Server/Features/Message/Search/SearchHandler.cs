using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Search;
using ServiceBusDriver.Core.Models.Features.Search;
using ServiceBusDriver.Server.Services.Validations;
using ServiceBusDriver.Shared.Features.Message;

namespace ServiceBusDriver.Server.Features.Message.Search
{
    public class SearchHandler : IRequestHandler<SearchRequest, List<MessageResponseDto>>
    {
        private readonly ISearchService _searchService;
        private readonly IMapper _mapper;
        private readonly IDbFetchHelper _dbFetchHelper;
        private readonly ILogger<SearchHandler> _logger;

        public SearchHandler(ISearchService searchService, ILogger<SearchHandler> logger, IMapper mapper, IDbFetchHelper dbFetchHelper)
        {
            _searchService = searchService;
            _logger = logger;
            _mapper = mapper;
            _dbFetchHelper = dbFetchHelper;
        }

        public async Task<List<MessageResponseDto>> Handle(SearchRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            await _dbFetchHelper.BelongToCurrentUser(request.InstanceId, cancellationToken);

            var searchCommand = MapToSearchCommand(request);

            var result = await _searchService.Search(searchCommand, cancellationToken);

            var response = _mapper.Map<List<MessageResponseDto>>(result);

            _logger.LogTrace("Finish {0}", nameof(Handle));

            return response;
        }

        private SearchCommand MapToSearchCommand(SearchRequest request)
        {
            var searchCommand = new SearchCommand
            {
                InstanceId = request.InstanceId,
                TopicName = request.TopicName,
                SubscriptionName = request.SubscriptionName,
                SearchDeadLetter = request.SearchDeadLetter,
                KeyPath = request.KeyPath,
                Value = request.Value,
                MatchType = request.MatchType,
                ContentType = request.ContentType,
                PrefetchCount = request.PrefetchCount,
                MaxMessages = 0
            };
            return searchCommand;
        }
    }
}