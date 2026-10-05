using FluentValidation;

namespace EventFlow.Event.Application.Features.EventPages.Commands.PublishEventPage;

public sealed class PublishEventPageCommandValidator
    : AbstractValidator<PublishEventPageCommand>
{
    public PublishEventPageCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Page ID is required.");
    }
}
