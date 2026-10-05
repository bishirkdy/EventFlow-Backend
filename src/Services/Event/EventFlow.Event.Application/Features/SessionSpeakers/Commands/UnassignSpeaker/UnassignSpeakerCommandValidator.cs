using FluentValidation;

namespace EventFlow.Event.Application.Features.SessionSpeakers.Commands.UnassignSpeaker;

public sealed class UnassignSpeakerCommandValidator : AbstractValidator<UnassignSpeakerCommand>
{
    public UnassignSpeakerCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.SessionId)
            .NotEmpty()
            .WithMessage("Session ID is required.");

        RuleFor(x => x.SpeakerId)
            .NotEmpty()
            .WithMessage("Speaker ID is required.");
    }
}
