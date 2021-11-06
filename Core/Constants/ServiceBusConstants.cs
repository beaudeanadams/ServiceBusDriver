namespace ServiceBusDriver.Core.Constants
{
    public static class ServiceBusConstants
    {

        public const string ConnectionStringEndpoint = "endpoint";
        public const string ConnectionStringSharedAccessKeyName = "sharedaccesskeyname";
        public const string ConnectionStringSharedAccessKey = "sharedaccesskey";
        public const string ConnectionStringStsEndpoint = "stsendpoint";
        public const string ConnectionStringTransportType = "transporttype";
        public const string ConnectionStringEntityPath = "entitypath";

       public const string ServiceBusNamespaceSharedAccessKeyIsInvalid = "The SharedAccessKey for the service bus namespace {0} is invalid.";
    }
}