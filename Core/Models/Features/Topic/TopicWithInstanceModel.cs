using System.Collections.Generic;
using ServiceBusDriver.Core.Models.Features.Instance;

namespace ServiceBusDriver.Core.Models.Features.Topic
{
    public class TopicWithInstanceModel
    {
        public InstanceResponse Instance { get; set; }
        public int TopicsCount { get; set; }
        public List<TopicResponse> Topics { get; set; }
    }
}