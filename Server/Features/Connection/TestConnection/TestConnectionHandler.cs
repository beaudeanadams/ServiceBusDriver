using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;
using MediatR;
using ServiceBusDriver.Core.Constants;
using ServiceBusDriver.Core.Models.Features.Connection;
using ServiceBusDriver.Core.Tools;
using ServiceBusDriver.Shared.Features.Error;
using ServiceBusDriver.Shared.Models;

namespace ServiceBusDriver.Server.Features.Connection.TestConnection
{
    public class TestConnectionHandler : IRequestHandler<TestConnectionRequest, ConnectionSettingsModel>
    {
        public async Task<ConnectionSettingsModel> Handle(TestConnectionRequest request, CancellationToken cancellationToken)
        {
            var sb = await Task.FromResult(
                    ServiceBusNamespaceTool.GetServiceBusNamespace(request.ConnectionString))
                .ConfigureAwait(false);


            var client = new ServiceBusAdministrationClient(request.ConnectionString, new ServiceBusAdministrationClientOptions
            {
                Retry = { MaxRetries = 1 }
            });

            try
            {
                var response = await client.GetNamespacePropertiesAsync(cancellationToken);

                var connectionSettingsModel = new ConnectionSettingsModel
                {
                    Uri = sb.Uri,
                    Namespace = sb.Namespace,
                    EntityPath = sb.EntityPath,
                    SharedAccessKeyName = sb.SharedAccessKeyName,
                    SharedAccessKey = sb.SharedAccessKey,
                    ConnectivityMode = null,
                    ConnectivityModeTransportType = sb.TransportType.ToString(),
                    Sku = response.Value.MessagingSku.ToString()
                };
                return connectionSettingsModel;
            }
            catch (Exception e)
            {
                throw new AppException(e.Message)
                {
                    ErrorMessage = new AppErrorMessageDto
                    {
                        Code = ErrorConstants.CommunicationsErrorCode,
                        UserMessageText = e.Message,
                        SupportReferenceId = new Guid().ToString()
                    }
                };
            }
        }
    }
}