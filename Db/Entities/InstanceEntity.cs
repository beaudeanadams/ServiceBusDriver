namespace ServiceBusDriver.Db.Entities
{
    public class InstanceEntity : BaseEntity
    {

        public string RawConnectionString { get; set; }

        public string Uri { get; set; }

        public string Namespace { get; set; }

        public string ServicePath { get; set; }

        public string TransportType { get; set; }

        public string StsEndpoint { get; set; }

        public string RuntimePort { get; set; }

        public string ManagementPort { get; set; }

        public string SharedAccessKeyName { get; set; }

        public string SharedAccessKey { get; set; }

        public string EntityPath { get; set; }

        public string UserId { get; set; }

    }
}
