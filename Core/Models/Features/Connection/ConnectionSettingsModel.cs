namespace ServiceBusDriver.Core.Models.Features.Connection
{
    public class ConnectionSettingsModel
    {
        public string Uri { get; set; }
        public string Namespace { get; set; }
        public string EntityPath { get; set; }
        public string SharedAccessKeyName { get; set; }
        public string SharedAccessKey { get; set; }
        public string ConnectivityMode { get; set; }
        public string ConnectivityModeTransportType { get; set; }
        public string Sku { get; set; }
    }
}