using System.Collections.Generic;
using Azure.Messaging.ServiceBus.Administration;

namespace ServiceBusDriver.Core.Models.Features.Subscription
{
    public class SubscriptionResponse
    {
        public SubscriptionProperties SubscriptionProperties { get; set; }
        public SubscriptionRuntimeProperties RunTimeProperties { get; set; }
        public List<RuleProperties> Rules { get; set; }
    }
}