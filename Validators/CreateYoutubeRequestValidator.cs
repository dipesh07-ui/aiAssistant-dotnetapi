using aiAssistant.api.DTOs;
using FluentValidation;

namespace aiAssistant.api.Validators
{
    public class CreateYoutubeRequestValidator : AbstractValidator<CreateYoutubeRequest>
    {
        public CreateYoutubeRequestValidator()
        {
            RuleFor(x => x.Url)
            .NotEmpty()
            .Must(url => url.Contains("youtube.com") ||
                         url.Contains("youtu.be"))
            .WithMessage("Must be a valid YouTube URL");
        }
    }
}
