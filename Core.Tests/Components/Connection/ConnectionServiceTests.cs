using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceBusDriver.Core.Components.Connection;
using ServiceBusDriver.Core.Components.SbClients;
using ServiceBusDriver.Core.Models.Features.Connection;
using ServiceBusDriver.Core.Models.Features.Instance;
using ServiceBusDriver.Core.Tests.TestData;
using ServiceBusDriver.Core.Tests.TestHelpers;
using Shouldly;
using Xunit;

namespace ServiceBusDriver.Core.Tests.Components.Connection
{
    public class ConnectionServiceTests
    {

        private readonly Mock<ISbAdminService> _adminService;
        private readonly Mock<ILogger<ConnectionService>> _logger;
        private readonly Mock<ServiceBusAdministrationClient> _serviceBusAdminClient;
        private readonly ConnectionService _connectionService;

        public ConnectionServiceTests()
        {
            _adminService = new Mock<ISbAdminService>();
            _logger = new Mock<ILogger<ConnectionService>>();
            _serviceBusAdminClient = new Mock<ServiceBusAdministrationClient>();

            _adminService.Setup(x => x.Client(It.IsAny<string>())).Returns(_serviceBusAdminClient.Object);
           

            _connectionService = new ConnectionService(_adminService.Object, _logger.Object);
        }

        [Fact]
        public async Task ProcessConnectionString_WorksAsExpected_ForValid_ConnectionString()
        {

            var connectionString = ConnectionTestData.ValidConnectionStrings[0];
           

            var result = await _connectionService.ProcessConnectionString(connectionString);

            result.Uri.ShouldBe("sb://sb-sbdriver-dev.servicebus.windows.net/");
            result.EntityPath.ShouldBe(string.Empty);
            result.Namespace.ShouldBe("sb-sbdriver-dev");
            result.TransportType.ToString().ShouldBe("AmqpTcp");
            result.SharedAccessKey.ShouldBe("aaaabbbbbbbbbbbccccccccccccddddddddddd/eeeeeee=");
            result.SharedAccessKeyName.ShouldBe("KeyForRead");
        }

    }
}
