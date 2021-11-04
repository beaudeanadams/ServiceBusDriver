using System;

namespace ServiceBusDriver.Shared.Features.Error
{
    public interface IAppException
    {
        AppErrorMessageDto ErrorMessage { get; }
    }

    public class AppException : Exception, IAppException
    {
        public AppException(string message) : base(message)
        {
        }

        public AppException()
        {
        }

        public int? HttpStatusCode { get; set; }
        public AppErrorMessageDto ErrorMessage { get; set; }
    }
}