using FluentValidation;

namespace EventFlow.Event.Application.Features.Events.Commands.CancelEvent;

internal sealed class CancelEventCommandValidator
    : AbstractValidator<CancelEventCommand>
{
    public CancelEventCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
