using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Server.Services.AuthContext;
using ServiceBusDriver.Server.Services.Validations;
using ServiceBusDriver.Shared.Features.Instance;

namespace ServiceBusDriver.Server.Features.Instance.Get
{
    public class GetHandler : IRequestHandler<GetRequestDto, InstanceResponseDto>
    {
        private readonly IDbFetchHelper _dbFetchHelper;
        private readonly ILogger<GetHandler> _logger;
        private readonly IMapper _mapper;

        public GetHandler(IDbFetchHelper dbFetchHelper, ICurrentUser currentUser, IMapper mapper, ILogger<GetHandler> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _dbFetchHelper = dbFetchHelper;
        }

        public async Task<InstanceResponseDto> Handle(GetRequestDto request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            var instance = await _dbFetchHelper.FetchIfBelongsToCurrentUser(request.Id, cancellationToken);

            var response = _mapper.Map<InstanceResponseDto>(instance);

            _logger.LogTrace("Finish {0}", nameof(Handle));

            return response;
        }
    }
}