using MediatR;
using ServiceBusDriver.Core.Models.Features.Connection;

namespace ServiceBusDriver.Shared.Models
{
    public class TestConnectionRequest : IRequest<ConnectionSettingsModel>
    {
        public string ConnectionString { get; set; }
    }
}