namespace ServiceBusDriver.Shared.Constants
{
    public class AppErrorConstants
    {
        public const string BadRequestErrorCode = "AERR00001";
        public const string BadRequestErrorMessage = "Bad Request. Check input parameters";

        public const string CommunicationsErrorCode = "AERR99999";
        public const string CommunicationsErrorMessage = "Unknown Error Occured";

        public const string ForbiddenErrorCode = "AERR00002";
        public const string ForbiddenErrorCodeMessage = "Forbidden";

        public const string AuthenticationErrorCode = "AERR00003";
        public const string AuthenticationErrorMessage = "Authentication Failed";

        public const string EncryptionFailureCode = "AERR00004";
        public const string EncryptionFailureMessage = "Encryption Failed";

        public const string DecryptionFailureCode = "AERR00005";
        public const string DecryptionFailureMessage = "Decryption Failed";

    }
}