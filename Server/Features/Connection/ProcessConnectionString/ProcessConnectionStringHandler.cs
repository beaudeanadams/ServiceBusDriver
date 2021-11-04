using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Core.Components.Connection;
using ServiceBusDriver.Core.Models.Features.Connection;
using ServiceBusDriver.Shared.Models;

namespace ServiceBusDriver.Server.Features.Connection.ProcessConnectionString
{
    public class ProcessConnectionStringHandler : IRequestHandler<ProcessConnectionStringRequest, ConnectionSettingsModel>
    {
        private readonly IConnectionService _connectionService;
        private readonly ILogger<ProcessConnectionStringHandler> _logger;

        public ProcessConnectionStringHandler(IConnectionService connectionService, ILogger<ProcessConnectionStringHandler> logger)
        {
            _connectionService = connectionService;
            _logger = logger;
        }

        public async Task<ConnectionSettingsModel> Handle(ProcessConnectionStringRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Start {0}", nameof(Handle));

            var sb = await _connectionService.ProcessConnectionString(request.ConnectionString);

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

            _logger.LogTrace("Finish {0}", nameof(Handle));

            return connectionSettingsModel;
        }
    }
}