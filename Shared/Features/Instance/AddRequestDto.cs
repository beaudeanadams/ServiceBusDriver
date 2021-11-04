using MediatR;

namespace ServiceBusDriver.Shared.Features.Instance
{
    public class AddRequestDto : IRequest<InstanceResponseDto>
    {
        public string ConnectionString { get; set; }
    }
}