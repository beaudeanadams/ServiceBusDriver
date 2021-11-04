using MediatR;

namespace ServiceBusDriver.Shared.Features.User
{
    public class VerifyOtpRequest : IRequest<Unit>
    {
        public string Id { get; set; }
        public string Otp { get; set; }

    }
}
