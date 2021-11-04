using FluentValidation;
using ServiceBusDriver.Shared.Tools;

namespace ServiceBusDriver.Shared.Features.User.Validators
{
    public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MaximumLength(50).IsValidPassword();
        }
    }
}