using FluentValidation;

namespace EventFlow.Event.Application.Features.EventPages.Commands.EnsureEventWebsite;

public sealed class EnsureEventWebsiteCommandValidator
    : AbstractValidator<EnsureEventWebsiteCommand>
{
    public EnsureEventWebsiteCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
