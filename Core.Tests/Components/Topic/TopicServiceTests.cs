using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceBusDriver.Core.Components.Instance;
using ServiceBusDriver.Core.Components.SbClients;
using ServiceBusDriver.Core.Components.Topic;
using System.Threading.Tasks;
using Xunit;

namespace ServiceBusDriver.Core.Tests.Components.Topic
{
    public class TopicServiceTests
    {
        private readonly Mock<ISbAdminService> _sbAdminService;
        private readonly Mock<ServiceBusAdministrationClient> _serviceBusAdminClient;
        private readonly IInstanceService _instanceService;
        private readonly ITopicService _topicService;
        private readonly Mock<ILogger<TopicService>> _logger;
        private readonly Mock<ILogger<InstanceService>> _instanceServiceLogger;

        public TopicServiceTests()
        {
            _logger = new Mock<ILogger<TopicService>>();
            _sbAdminService = new Mock<ISbAdminService>();
            _instanceServiceLogger = new Mock<ILogger<InstanceService>>();
            _instanceService = new InstanceService(_instanceServiceLogger.Object);
            _serviceBusAdminClient = new Mock<ServiceBusAdministrationClient>();

            _topicService = new TopicService(_sbAdminService.Object, _instanceService, _logger.Object);

        }

        [Fact]
        public async Task GetTopicsForInstance_Valid_ReturnsTestInstances()
        {
            //TODO Add workaround as there is no default constructor with empty params for TopicProperty
            // var topicPropertiesJson = FileHelpers.ReadResource("ServiceBusDriver.Core.Tests.TestData.topicProperties.json");
            // var topicRuntimePropertiesJson = FileHelpers.ReadResource("ServiceBusDriver.Core.Tests.TestData.topicRuntimeProperties.json");
            //
            // var topicProperties = JsonConvert.DeserializeObject<List<TopicProperties>>(topicPropertiesJson, new JsonSerializerSettings
            // {
            //     ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            // });
            // var topicRuntimeProperties = JsonConvert.DeserializeObject<List<TopicRuntimeProperties>>(topicRuntimePropertiesJson);
            //
            // _sbAdminService.Setup(x => x.Client(It.IsAny<string>()))
            //                .Returns(_serviceBusAdminClient.Object);
            //
            // _serviceBusAdminClient.Setup(x => x.GetTopicsAsync(It.IsAny<CancellationToken>()).GetItems())
            //                       .ReturnsAsync(topicProperties);
            //
            // _serviceBusAdminClient.Setup(x => x.GetTopicsRuntimePropertiesAsync(It.IsAny<CancellationToken>()).GetItems())
            //                       .ReturnsAsync(topicRuntimeProperties);
            //
            //
            // var result = await _topicService.GetTopicsForInstance(DemoDataConstants.DummyInstances[0].Id);
            //
            // var expected = FileHelpers.ReadResource("ServiceBusDriver.Core.Tests.TestData.topicResponseDto.Expected.json");
            // var actual = JsonConvert.SerializeObject(result);
            //
            // actual.ShouldBe(expected);
        }
    }
}
