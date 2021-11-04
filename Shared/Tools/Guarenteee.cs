using System;
using AppException = ServiceBusDriver.Shared.Features.Error.AppException;
using AppErrorConstants = ServiceBusDriver.Shared.Constants.AppErrorConstants;
using ErrorMessageModel = ServiceBusDriver.Shared.Features.Error.AppErrorMessageDto;

namespace ServiceBusDriver.Shared.Tools
{
    public class Guarantee
    {
        public const string BadRequestErrorCode = AppErrorConstants.BadRequestErrorCode;

        public static void NotNull(object value, string name)
        {
            That<AppException>(value != null, $"{name} cannot be null or empty");
        }

        public static void NotNull(object value)
        {
            NotNull(value, nameof(value));
        }

        public static void That<TException>(bool assertion, string message) where TException : Exception
        {
            if (assertion == false)
                throw new AppException
                {
                    ErrorMessage = new ErrorMessageModel()
                    {
                        Code = BadRequestErrorCode,
                        UserMessageText = message ?? "Payload Validation Failed"
                    }
                };
        }

        public static void That<TException>(Func<bool> assertion, string message) where TException : Exception
        {
            if (assertion() == false) throw (TException)Activator.CreateInstance(typeof(TException), message);
        }
    }
}