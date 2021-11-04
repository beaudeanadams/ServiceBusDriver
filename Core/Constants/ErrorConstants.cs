namespace ServiceBusDriver.Core.Constants
{
    public class ErrorConstants
    {
        public const string BadRequestErrorCode = "AERR00001";
        public const string BadRequestErrorMessage = "Bad Request. Check input parameters";

        public const string CommunicationsErrorCode = "AERR99999";
        public const string CommunicationsErrorMessage = "Unknown Error Occured";

        public const string ForbiddenErrorCode = "AERR00002";
        public const string ForbiddenErrorCodeMessage = "Forbidden";

        public const string NoMessagesInQueueErrorCode = "AERR00003";
        public const string NoMessagesInQueueErrorMessage = "No Messages in {0}. Search needs atleast one message in {0}";
    }
}