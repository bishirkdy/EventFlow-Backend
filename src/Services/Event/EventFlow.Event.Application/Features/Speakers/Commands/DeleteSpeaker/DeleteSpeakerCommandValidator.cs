using FluentValidation;

namespace EventFlow.Event.Application.Features.Speakers.Commands.DeleteSpeaker;

public sealed class DeleteSpeakerCommandValidator : AbstractValidator<DeleteSpeakerCommand>
{
    public DeleteSpeakerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Speaker ID is required.");

        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
