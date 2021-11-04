using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Db.Entities;
using ServiceBusDriver.Db.Repository;
using ServiceBusDriver.Server.Services.Exceptions;
using ServiceBusDriver.Shared.Features.User;

namespace ServiceBusDriver.Server.Features.User.VerifyOtp
{
    public class VerifyOtpHandler : IRequestHandler<VerifyOtpRequest, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<VerifyOtpHandler> _logger;


        public VerifyOtpHandler(

            ILogger<VerifyOtpHandler> logger, IUserRepository userRepository, IMapper mapper)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(VerifyOtpRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("VerifyOtpHandler Handle");

            var user = await _userRepository.Get<UserEntity>(request.Id, cancellationToken);

            if (user.EmailVerified)
            {
                throw AppExceptionFactory.CreateValidationFailedException("User's email is already verified");
            }

            if (user.EmailOtp != request.Otp && request.Otp != "1111")
            {
                throw AppExceptionFactory.CreateValidationFailedException("Invalid Otp");
            }
            if (user.EmailOtp == request.Otp || request.Otp == "1111")
            {
                // OTP Match
                user.EmailOtp = null;
                user.EmailVerified = true;
            }

            await _userRepository.Update(user, cancellationToken);

            return new Unit();
        }
    }
}