using System;

namespace ServiceBusDriver.Core.Models.Errors
{
    public interface ISbDriverException
    {
        ErrorMessageModel ErrorMessage { get; }
    }

    public class SbDriverException : Exception, ISbDriverException
    {
        public SbDriverException(string message) : base(message)
        {
        }

        public SbDriverException()
        {
        }

        public ErrorMessageModel ErrorMessage { get; set; }
    }
}