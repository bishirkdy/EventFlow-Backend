using FluentValidation;

namespace EventFlow.Event.Application.Features.Events.Commands.PublishEvent;

internal sealed class PublishEventCommandValidator : AbstractValidator<PublishEventCommand>
{
    public PublishEventCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
