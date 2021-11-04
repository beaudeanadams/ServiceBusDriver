using FluentValidation;
using ServiceBusDriver.Shared.Tools;

namespace ServiceBusDriver.Shared.Features.User.Validators
{
    public class AddRequestDtoValidator : AbstractValidator<AddRequestDto>
    {
        public AddRequestDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MaximumLength(50).IsValidPassword();
            RuleFor(x => x.TeamName).NotEmpty().MaximumLength(50);
        }
    }
}