namespace ServiceBusDriver.Core.Constants
{
    public static class ServiceBusConstants
    {
        public const string DefaultScheme = "sb";
        public const string MessageNumber = "MessageNumber";
        public const string StringType = "String";
        public const string DeadLetterQueue = "$DeadLetterQueue";
        public const string NullValue = "NULL";
        public const string CloudServiceBusPostfix = ".servicebus.windows.net";
        public const string GermanyServiceBusPostfix = ".servicebus.cloudapi.de";
        public const string ChinaServiceBusPostfix = ".servicebus.chinacloudapi.cn";
        public const string TestServiceBusPostFix = ".servicebus.int7.windows-int.net";
        public const int MaxBufferSize = 262144; // 256 KB

        public const string ConnectionStringEndpoint = "endpoint";
        public const string ConnectionStringSharedAccessKeyName = "sharedaccesskeyname";
        public const string ConnectionStringSharedAccessKey = "sharedaccesskey";
        public const string ConnectionStringStsEndpoint = "stsendpoint";
        public const string ConnectionStringRuntimePort = "runtimeport";
        public const string ConnectionStringManagementPort = "managementport";
        public const string ConnectionStringWindowsUsername = "windowsusername";
        public const string ConnectionStringWindowsDomain = "windowsdomain";
        public const string ConnectionStringWindowsPassword = "windowspassword";
        public const string ConnectionStringTransportType = "transporttype";
        public const string ConnectionStringEntityPath = "entitypath";

        public const string ServiceBusNamespacesNotConfigured = "Service bus accounts have not been properly configured in the configuration file.";
        public const string ServiceBusNamespaceIsNullOrEmpty = "The connection string for service bus entry {0} is null or empty.";
        public const string ServiceBusNamespaceIsWrong = "The connection string for service bus namespace {0} is in the wrong format.";
        public const string ServiceBusNamespaceEndpointIsNullOrEmpty = "The endpoint for the service bus namespace {0} is null or empty.";
        public const string ServiceBusNamespaceEndpointPrefixedWithSb = "The endpoint for the service bus namespace {0} is being automatically prefixed with \"sb://\".";
        public const string ServiceBusNamespaceStsEndpointIsNullOrEmpty = "The sts endpoint for the service bus namespace {0} is null or empty.";
        public const string ServiceBusNamespaceRuntimePortIsNullOrEmpty = "The runtime port for the service bus namespace {0} is null or empty.";
        public const string ServiceBusNamespaceManagementPortIsNullOrEmpty = "The management port for the service bus namespace {0} is null or empty.";
        public const string ServiceBusNamespaceEndpointUriIsInvalid = "The endpoint URI for the service bus namespace {0} is invalid.";
        public const string ServiceBusNamespaceSharedAccessKeyNameIsInvalid = "The SharedAccessKeyName for the service bus namespace {0} is invalid.";
        public const string ServiceBusNamespaceSharedAccessKeyIsInvalid = "The SharedAccessKey for the service bus namespace {0} is invalid.";
    }
}