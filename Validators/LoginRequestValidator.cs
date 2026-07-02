using aiAssistant.api.DTOs;
using FluentValidation;

namespace aiAssistant.api.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(8);
        }
    }
}
