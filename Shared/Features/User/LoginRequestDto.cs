using MediatR;

namespace ServiceBusDriver.Shared.Features.User
{
    public class LoginRequestDto : IRequest<LoginResponseDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}