using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceBusDriver.Client.Services
{
    public class PropertiesNotifierService
    {
        public IReadOnlyDictionary<string, string> TopicProperties { get; private set; } = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, string> SubscriptionProperties { get; private set; } = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, string> MessageProperties { get; private set; } = new Dictionary<string, string>();

        public PropertiesNotifierService()
        {

        }

        public async Task SetTopicProperties(Dictionary<string, string> properties)
        {
            TopicProperties = properties;

            if (Notify != null)
            {
                await Notify?.Invoke();
            }

        }
        
        public async Task SetSubscriptionProperties(Dictionary<string, string> properties)
        {
            SubscriptionProperties = properties;

            if (Notify != null)
            {
                await Notify?.Invoke();
            }

        }
        
        public async Task SetMessageProperties(Dictionary<string, string> properties)
        {
            MessageProperties = properties;

            if (Notify != null)
            {
                await Notify?.Invoke();
            }

        }

        public event Func<Task> Notify;
    }
}
