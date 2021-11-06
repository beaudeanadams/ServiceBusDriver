using ServiceBusDriver.Core.Models.Features.Instance;
using System.Collections.Generic;

namespace ServiceBusDriver.Core.Constants
{
    public class DemoDataConstants
    {
        public static List<ServiceBusInstanceModel> DummyInstances = new List<ServiceBusInstanceModel>
        {
            new()
            {
                Id = "8e495aaa-7bd8-44ea-83f5-7c5d166ee7ff",
                Name = "dev-sbd-qqq-sb01",
                ConnectionString =
                    "Endpoint=sb://dev-sbd-qqq-sb01.servicebus.windows.net/;SharedAccessKeyName=RootKey;SharedAccessKey=sads+sdfsdfsdfsdfsdfsdfasdasdad="
            },
            new()
            {
                Id = "7582be0d-aebc-4a05-9734-451c712891e9",
                Name = "sit-sbd-qqq-sb01",
                ConnectionString =
                    "Endpoint=sb://sit-sbd-qqq-sb01.servicebus.windows.net/;SharedAccessKeyName=RootKey;SharedAccessKey=ddfdf+sdfsdfsdfasdfsdfasdfsdfsdf="
            }
        };
    }
}
