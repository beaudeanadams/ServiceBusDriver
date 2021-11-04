using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Server.Services.AuthContext;
using ServiceBusDriver.Shared.Features.User;

namespace ServiceBusDriver.Server.Features.User.GetMe
{
    public class GetMeHandler : IRequestHandler<GetMeRequestDto, UserResponseDto>
    {

        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<GetMeHandler> _logger;

        public GetMeHandler(ILogger<GetMeHandler> logger, IMapper mapper, ICurrentUser currentUser)
        {
            _logger = logger;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<UserResponseDto> Handle(GetMeRequestDto request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));


            var result = await Task.FromResult(_currentUser.User);

            var response = _mapper.Map<UserResponseDto>(result);

            _logger.LogTrace("Finish {0}", nameof(Handle));

            return response;
        }
    }
}