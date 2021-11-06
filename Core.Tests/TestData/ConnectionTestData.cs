using System.Collections.Generic;
using Microsoft.Azure.Amqp.Serialization;

namespace ServiceBusDriver.Core.Tests.TestData
{
    internal class ConnectionTestData
    {
        public static readonly List<string> ValidConnectionStrings = new()
        {
            "Endpoint=sb://sb-sbdriver-dev.servicebus.windows.net/;SharedAccessKeyName=KeyForRead;SharedAccessKey=aaaabbbbbbbbbbbccccccccccccddddddddddd/eeeeeee="
        };
    }
}
