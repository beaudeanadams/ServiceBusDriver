using Azure.Messaging.ServiceBus;

namespace ServiceBusDriver.Core.Models.Features.Instance
{
    public enum ServiceBusNamespaceType
    {
        Custom,
        Cloud,
        OnPremises
    }

    public class ServiceBusNamespaceModel
    {
        public ServiceBusNamespaceType ConnectionStringType { get; set; }
        public string ConnectionString { get; set; }
        public string Uri { get; set; }
        public string Namespace { get; set; }
        public string ServicePath { get; set; }
        public ServiceBusTransportType TransportType { get; set; }
        public string StsEndpoint { get; set; }
        public string RuntimePort { get; set; }
        public string ManagementPort { get; set; }
        public string SharedAccessKeyName { get; set; }
        public string SharedAccessKey { get; set; }
        public string EntityPath { get; set; }
    }
}