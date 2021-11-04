using ServiceBusDriver.Core.Constants;
using ServiceBusDriver.Core.Models.Errors;

namespace ServiceBusDriver.Core
{
    public class SbDriverExceptionFactory
    {

        public static SbDriverException CreateBadRequestException(string message)
        {
            return new SbDriverException(message)
            {
                ErrorMessage = new ErrorMessageModel
                {
                    Code = ErrorConstants.BadRequestErrorCode,
                    UserMessageText = message ?? ErrorConstants.BadRequestErrorMessage
                }
            };
        }

        public static SbDriverException CreateValidationFailedException(string message)
        {
            return new SbDriverException(message)
            {
                ErrorMessage = new ErrorMessageModel
                {
                    Code = ErrorConstants.BadRequestErrorCode,
                    UserMessageText = message
                }
            };
        }
    }
}