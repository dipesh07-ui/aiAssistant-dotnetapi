using aiAssistant.api.DTOs;
using FluentValidation;

namespace aiAssistant.api.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Must contain uppercase")
                .Matches("[0-9]").WithMessage("Must contain number");
        }
    }
}
