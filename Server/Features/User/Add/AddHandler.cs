using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Db.Entities;
using ServiceBusDriver.Db.Repository;
using ServiceBusDriver.Server.Services;
using ServiceBusDriver.Server.Services.Email;
using ServiceBusDriver.Server.Services.FirebaseAuth;
using ServiceBusDriver.Server.Services.Validations;
using ServiceBusDriver.Shared.Constants;
using ServiceBusDriver.Shared.Features.Error;
using ServiceBusDriver.Shared.Features.User;

namespace ServiceBusDriver.Server.Features.User.Add
{
    public class AddHandler : IRequestHandler<AddRequestDto, UserResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IDisposableEmailChecker _disposableEmailChecker;
        private readonly IMapper _mapper;
        private readonly IFirebaseAuthManager _firebaseAuthManager;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ILogger<AddHandler> _logger;

        public AddHandler(IUserRepository userRepository, ILogger<AddHandler> logger, IMapper mapper,
                          IFirebaseAuthManager firebaseAuthManager, IEmailSenderService emailSenderService, IDisposableEmailChecker disposableEmailChecker)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
            _firebaseAuthManager = firebaseAuthManager;
            _emailSenderService = emailSenderService;
            _disposableEmailChecker = disposableEmailChecker;
        }

        public async Task<UserResponseDto> Handle(AddRequestDto requestDto, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            await _disposableEmailChecker.CheckIfDisposableEmailDomain(requestDto.Email, cancellationToken);
            
            var firebaseAuthClient = _firebaseAuthManager.GetClient();

            var emailCheck = await firebaseAuthClient.FetchSignInMethodsForEmailAsync(requestDto.Email);

            if (!emailCheck.UserExists)
            {
                var userCredential = await _firebaseAuthManager.GetClient().CreateUserWithEmailAndPasswordAsync(requestDto.Email, requestDto.Password);

                var result = await _userRepository.Add(new UserEntity
                {
                    TeamName = requestDto.TeamName,
                    AuthUserId = userCredential.User.Uid,
                    Email = requestDto.Email,
                    EmailOtp = OtpHelper.GenerateOtp(),
                    EmailVerified = false
                }, cancellationToken);

                var response = _mapper.Map<UserResponseDto>(result);
                _logger.LogTrace("Finish {0}", nameof(Handle));

                SendEmail(result, cancellationToken);

                return response;
            }
            else
            {
                throw new AppException()
                {
                    HttpStatusCode = StatusCodes.Status400BadRequest,
                    ErrorMessage = new AppErrorMessageDto
                    {
                        Code = AppErrorConstants.BadRequestErrorCode,
                        UserMessageText = "Email already in system"
                    }
                };
            }
        }

        private void SendEmail(UserEntity user, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Sending Email");

            _emailSenderService.SendCodeEmail(user.TeamName, user.Email,
                                                             user.EmailOtp,
                                                             cancellationToken).SafeFireAndForget();
        }
    }
}