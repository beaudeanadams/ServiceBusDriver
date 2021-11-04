using MediatR;

namespace ServiceBusDriver.Shared.Features.User
{
    public class GetRequestDto : IRequest<UserResponseDto>
    {
        public GetRequestDto(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}