using System;
using System.Linq;
using System.Threading;
using ServiceBusDriver.Core.Constants;
using ServiceBusDriver.Db.Entities;
using ServiceBusDriver.Db.Repository;
using ServiceBusDriver.Shared.Features.Error;

namespace ServiceBusDriver.Server.Services.AuthContext
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IUserRepository _userRepository;
        private readonly IClaimsManager _claimsManager;
        private UserEntity _user;

        public CurrentUser(IUserRepository userRepository, IClaimsManager claimsManager)
        {
            _userRepository = userRepository;
            _claimsManager = claimsManager;
        }

        public UserEntity User
        {
            get
            {
                try
                {
                    if (_user == null)
                    {
                        var userId = _claimsManager.GetUserId();

                        try
                        {
                            _user = (_userRepository.GetUserWhereAuthUserIdIs(userId, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult()).FirstOrDefault();

                            if (_user == null) 
                                throw new AppException();

                            if (!_user.EmailVerified)
                            {
                                throw new AppException("Email not verified")
                                {
                                    HttpStatusCode = 403,
                                    ErrorMessage = new AppErrorMessageDto
                                    {
                                        Code = ErrorConstants.ForbiddenErrorCode,
                                        UserMessageText = ErrorConstants.ForbiddenErrorCodeMessage
                                    }
                                };
                            }
                        }
                        catch (Exception)
                        {
                            throw new AppException("Invalid Claim");
                        }
                    }

                    return _user;
                }
                catch (Exception)
                {
                    throw new AppException("Current User Not Found")
                    {
                        HttpStatusCode = 403,
                        ErrorMessage = new AppErrorMessageDto
                        {
                            Code = ErrorConstants.ForbiddenErrorCode,
                            UserMessageText = ErrorConstants.ForbiddenErrorCodeMessage
                        }
                    };
                }
            }
            set => _user = value;
        }
    }
}