using FluentValidation;

namespace EventFlow.Event.Application.Features.SessionSpeakers.Commands.AssignSpeaker;

public sealed class AssignSpeakerCommandValidator : AbstractValidator<AssignSpeakerCommand>
{
    public AssignSpeakerCommandValidator()
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
