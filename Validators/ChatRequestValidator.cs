using aiAssistant.api.DTOs;
using FluentValidation;

namespace aiAssistant.api.Validators
{
    public class ChatRequestValidator : AbstractValidator<ChatRequest>
    {
        public ChatRequestValidator()
        {
            RuleFor(x => x.Question)
                .NotEmpty().MaximumLength(2000);
        }
    }
}
