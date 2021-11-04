using MediatR;

namespace ServiceBusDriver.Shared.Features.User
{
    public class AddRequestDto : IRequest<UserResponseDto>
    {
        public AddRequestDto()
        {
        }

        public string TeamName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}