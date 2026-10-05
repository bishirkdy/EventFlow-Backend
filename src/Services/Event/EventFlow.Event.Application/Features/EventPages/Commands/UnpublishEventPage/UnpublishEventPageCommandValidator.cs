using FluentValidation;

namespace EventFlow.Event.Application.Features.EventPages.Commands.UnpublishEventPage;

public sealed class UnpublishEventPageCommandValidator
    : AbstractValidator<UnpublishEventPageCommand>
{
    public UnpublishEventPageCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Page ID is required.");
    }
}
