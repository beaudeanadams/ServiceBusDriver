using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Identity;
using Firebase.Auth;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Db.Repository;
using ServiceBusDriver.Server.Services.FirebaseAuth;
using ServiceBusDriver.Shared.Features.User;

namespace ServiceBusDriver.Server.Features.User.Login
{
    public class LoginHandler : IRequestHandler<LoginRequestDto, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IFirebaseAuthManager _firebaseAuthManager;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(IUserRepository userRepository, ILogger<LoginHandler> logger, IMapper mapper, IFirebaseAuthManager firebaseAuthManager)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
            _firebaseAuthManager = firebaseAuthManager;
        }

        public async Task<LoginResponseDto> Handle(LoginRequestDto request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            try
            {
                var firebaseClient = _firebaseAuthManager.GetClient();

                var signInMethodsForEmail = await firebaseClient.FetchSignInMethodsForEmailAsync(request.Email);

                if (signInMethodsForEmail.UserExists && signInMethodsForEmail.AllProviders.Contains(FirebaseProviderType.EmailAndPassword))
                {
                    _logger.LogInformation("User exists for email {0}", signInMethodsForEmail.Email);
                    try
                    {
                        var emailUser = await firebaseClient.SignInWithEmailAndPasswordAsync(request.Email, request.Password);

                        var userEntity = (await _userRepository.GetUserWhereAuthUserIdIs(emailUser.User.Uid, cancellationToken)).FirstOrDefault();

                        var response = new LoginResponseDto
                        {
                            TeamName = userEntity.TeamName,
                            Email = userEntity.Email,
                            Id = userEntity.Id,
                            AuthId = userEntity.AuthUserId,
                            Token = await emailUser.User.GetIdTokenAsync(true)
                        };

                        _logger.LogTrace("Finish {0}", nameof(Handle));

                        return response;
                    }
                    catch (FirebaseAuthException ex)
                    {
                        var errorMessage = ex.Reason.ToString();
                        throw new AuthenticationFailedException(errorMessage);
                    }
                }
                else
                {
                    throw new AuthenticationFailedException("User does not exist in system OR signInMethod not supported");
                }
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new AuthenticationFailedException("Cannot authenticate user");
            }
        }
    }
}