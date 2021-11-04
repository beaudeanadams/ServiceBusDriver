using Newtonsoft.Json;

namespace ServiceBusDriver.Db.Entities
{
    public class BaseEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}