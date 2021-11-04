using MediatR;

namespace ServiceBusDriver.Shared.Features.Instance
{
    public class GetRequestDto : IRequest<InstanceResponseDto>
    {
        public GetRequestDto(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}