using ServiceBusDriver.Core.Tools;
using Shouldly;
using Xunit;

namespace ServiceBusDriver.Core.Tests.Tools
{
    public class JsonExtensionsTests
    {
        [Fact]
        public void IsJson_ReturnsSuccessForJsonString()
        {
            "{\"key\":\"value\",\"arrayKey\":[\"hjk\",\"sdsd\",\"sdsdwewe\"]}".IsJson().ShouldBeTrue();
        }

        [Fact]
        public void IsJson_ReturnsErrorForNonJsonString()
        {
            "I am not json".IsJson().ShouldBeFalse();
        }
    }
}
