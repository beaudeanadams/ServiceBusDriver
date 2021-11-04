using System;

namespace ServiceBusDriver.Client.Services
{
    public interface ISbDriverUiException
    {
         public string UiError { get; set; }
         public string LogError { get; set; }
    }

    public class SbDriverUiException : Exception, ISbDriverUiException
    {
        public SbDriverUiException(string uiError) : base(uiError)
        {
            LogError = uiError;
        }

        public SbDriverUiException(string uiError, string logError) : base(logError)
        {
            UiError = uiError;
            LogError = logError;
        }

        
        public SbDriverUiException()
        {
        }

        public int? HttpStatusCode { get; set; }
        public string UiError { get; set; }
        public string LogError { get; set; }
    }
}