using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Db.Entities;
using ServiceBusDriver.Db.Repository;
using ServiceBusDriver.Server.Services.AuthContext;
using ServiceBusDriver.Server.Services.Exceptions;
using ServiceBusDriver.Shared.Features.User;

namespace ServiceBusDriver.Server.Features.User.Get
{
    public class GetHandler : IRequestHandler<GetRequestDto, UserResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<GetHandler> _logger;

        public GetHandler(IUserRepository userRepository, ILogger<GetHandler> logger, IMapper mapper, ICurrentUser currentUser)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<UserResponseDto> Handle(GetRequestDto request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            if (_currentUser.User.Id != request.Id)
            {
                throw AppExceptionFactory.CreateForbiddenException();
            }

            var result = await _userRepository.Get<UserEntity>(request.Id, cancellationToken);

            var response = _mapper.Map<UserResponseDto>(result);

            _logger.LogTrace("Finish {0}", nameof(Handle));

            return response;
        }
    }
}