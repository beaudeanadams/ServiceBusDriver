using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.SbClients;
using ServiceBusDriver.Core.Models.Features.Connection;
using ServiceBusDriver.Core.Models.Features.Instance;
using ServiceBusDriver.Core.Tools;

namespace ServiceBusDriver.Core.Components.Connection
{
    public class ConnectionService : IConnectionService
    {
        private readonly ISbAdminService _adminService;
        private readonly ILogger<ConnectionService> _logger;

        public ConnectionService(ISbAdminService adminService, ILogger<ConnectionService> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        /// <summary>
        /// Get ConnectionString Components
        /// </summary>
        ///  <param name="connectionString">Connection String</param>
        public async Task<ServiceBusNamespaceModel> ProcessConnectionString(string connectionString)
        {
            _logger.LogTrace("Start {0}", nameof(ProcessConnectionString));

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return null;
            }

            try
            {
                var sb = await Task.FromResult(
                        ServiceBusNamespaceTool.GetServiceBusNamespace(connectionString))
                    .ConfigureAwait(false);

                _logger.LogInformation("Processed ConnectionString with NameSpace {0}", sb.Namespace);

                _logger.LogTrace("Finish {0}", nameof(ProcessConnectionString));
                return sb;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error which processing ConnectionString {0}", connectionString);
                return null;
            }
        }

        /// <summary>
        /// Test Connection to connection string
        /// </summary>
        ///  <param name="connectionString">Connection String</param>
        ///  <param name="cancellationToken">cancellationToken</param>
        public async Task<ConnectionSettingsModel> TestConnection(string connectionString, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Start {0}", nameof(TestConnection));

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return null;
            }

            try
            {
                var response = await (_adminService.Client(connectionString)).GetNamespacePropertiesAsync(cancellationToken);

                var sb = await ProcessConnectionString(connectionString);

                var connectionSettingsModel = new ConnectionSettingsModel
                {
                    Uri = sb.Uri,
                    Namespace = sb.Namespace,
                    EntityPath = sb.EntityPath,
                    SharedAccessKeyName = sb.SharedAccessKeyName,
                    SharedAccessKey = sb.SharedAccessKey,
                    ConnectivityMode = null,
                    ConnectivityModeTransportType = sb.TransportType.ToString()
                };

                connectionSettingsModel.Sku = response.Value.MessagingSku.ToString();

                _logger.LogInformation("Tested Connection to NameSpace {0}", connectionSettingsModel.Namespace);

                _logger.LogTrace("Finish {0}", nameof(TestConnection));

                return connectionSettingsModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error which processing ConnectionString {0}", connectionString);
                return null;
            }
        }
    }
}