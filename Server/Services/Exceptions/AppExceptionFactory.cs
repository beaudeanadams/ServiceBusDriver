using Microsoft.AspNetCore.Http;
using ServiceBusDriver.Shared.Constants;
using ServiceBusDriver.Shared.Features.Error;

namespace ServiceBusDriver.Server.Services.Exceptions
{
    public class AppExceptionFactory
    {
        public static AppErrorMessageDto CreateBadRequestException()
        {
            return CreateBadRequestError(AppErrorConstants.BadRequestErrorMessage);
        }

        public static AppErrorMessageDto CreateBadRequestError(string message)
        {
            return new AppErrorMessageDto
            {
                Code = AppErrorConstants.BadRequestErrorCode,
                UserMessageText = message ?? AppErrorConstants.BadRequestErrorMessage
            };
        }

        public static AppException CreateBadRequestException(string message)
        {
            return new AppException(message)
            {
                ErrorMessage = new AppErrorMessageDto
                {
                    Code = AppErrorConstants.BadRequestErrorCode,
                    UserMessageText = message ?? AppErrorConstants.BadRequestErrorMessage
                }
            };
        }

        public static AppErrorMessageDto CreateAuthenticationError(string message)
        {
            return new AppErrorMessageDto
            {
                Code = AppErrorConstants.AuthenticationErrorCode,
                UserMessageText = message ?? AppErrorConstants.AuthenticationErrorMessage
            };
        }

        public static AppException CreateAuthenticationException(string message)
        {
            return new AppException(message)
            {
                ErrorMessage = new AppErrorMessageDto
                {
                    Code = AppErrorConstants.AuthenticationErrorCode,
                    UserMessageText = message ?? AppErrorConstants.AuthenticationErrorMessage
                }
            };
        }

        public static AppErrorMessageDto CreateServerCommunicationException()
        {
            return new AppErrorMessageDto
            {
                Code = AppErrorConstants.CommunicationsErrorCode,
                UserMessageText = AppErrorConstants.CommunicationsErrorMessage
            };
        }

        public static AppException CreateValidationFailedException(string message)
        {
            return new AppException(message)
            {
                ErrorMessage = new AppErrorMessageDto
                {
                    Code = AppErrorConstants.BadRequestErrorCode,
                    UserMessageText = message
                }
            };
        }

        public static AppException CreateForbiddenException()
        {
            return new AppException()
            {
                HttpStatusCode = StatusCodes.Status403Forbidden,
                ErrorMessage = new AppErrorMessageDto
                {
                    Code = AppErrorConstants.ForbiddenErrorCode,
                    UserMessageText = AppErrorConstants.ForbiddenErrorCodeMessage
                }
            };
        }
    }
}