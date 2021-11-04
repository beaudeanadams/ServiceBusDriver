using System.Text.RegularExpressions;
using FluentValidation;

namespace ServiceBusDriver.Shared.Tools
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, string> IsValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return (IRuleBuilderOptions<T, string>) ruleBuilder.Custom((value, context) =>
            {
                var error = $"Entered value is not valid Password. Add 1 uppercase, 1 lowercase, 1 number and a symbol(@#$%) with min 8 character length";

                if (value.Length < 8)
                    context.AddFailure(error);

                var r = new Regex("^((?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{8,50})$");

                if (!r.IsMatch(value))
                    context.AddFailure(error);
            });
        }
    }
}