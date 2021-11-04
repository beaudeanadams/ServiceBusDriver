using System;
using System.Text;

namespace ServiceBusDriver.Shared.Tools
{
    public static class ExceptionExtensions
    {
        public static string ToFullExceptionString(this Exception exception)
        {
            return ToFullExceptionString(exception, string.Empty);
        }

        private static string ToFullExceptionString(this Exception exception, string additionalMessage)
        {
            try
            {
                if (exception == null) return additionalMessage + "A null exception was thrown";

                try
                {
                    var errorBuilder = new StringBuilder();

                    if (!string.IsNullOrWhiteSpace(additionalMessage)) errorBuilder.AppendLine(additionalMessage);

                    var exceptionLayer = exception;

                    while (exceptionLayer != null)
                    {
                        errorBuilder.AppendLine(exceptionLayer.ToString());
                        errorBuilder.AppendLine(exceptionLayer.Source);
                        errorBuilder.AppendLine();
                        errorBuilder.AppendLine();
                        exceptionLayer = exceptionLayer.InnerException;
                    }

                    return errorBuilder.ToString();
                }
                catch
                {
                    return exception.ToString();
                }
            }
            catch
            {
                return "An exception was thrown but we were unable to parse it";
            }
        }
    }
}