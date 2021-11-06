using Microsoft.Extensions.Logging;
using Moq;
using ServiceBusDriver.Core.Components.Instance;
using Shouldly;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceBusDriver.Core.Constants;
using Xunit;

namespace ServiceBusDriver.Core.Tests.Components.Instance
{
    public class InstanceServiceTests
    {
        private readonly Mock<ILogger<InstanceService>> _logger;
        private readonly InstanceService _instanceService;

        public InstanceServiceTests()
        {
            _logger = new Mock<ILogger<InstanceService>>();
            _instanceService = new InstanceService(_logger.Object);
        }

        [Fact]
        public async Task ListInstancesFull_Valid_ReturnsTestInstances()
        {
            var result = await _instanceService.ListInstancesFull();

            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);

            var expected = JsonConvert.SerializeObject(DemoDataConstants.DummyInstances);
            var actual = JsonConvert.SerializeObject(result);

            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task GetInstanceFull_Valid_ReturnsTestInstances()
        {
            var result = await _instanceService.GetInstanceFull(DemoDataConstants.DummyInstances[0].Id);

            result.ShouldNotBeNull();

            var expected = JsonConvert.SerializeObject(DemoDataConstants.DummyInstances[0]);
            var actual = JsonConvert.SerializeObject(result);

            actual.ShouldBe(expected);
        }
    }
}
