using EventFlow.Event.Domain.Enums;
using FluentValidation;

namespace EventFlow.Event.Application.Features.Feedback.Commands.SubmitFeedback;

public sealed class SubmitFeedbackCommandValidator : AbstractValidator<SubmitFeedbackCommand>
{
    public SubmitFeedbackCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.ParticipantUserId)
            .NotEmpty()
            .WithMessage("Participant user ID is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .WithMessage("Comment cannot exceed 2000 characters.");

        RuleFor(x => x.TargetId)
            .NotEmpty()
            .When(x => x.TargetType != FeedbackTargetType.Event)
            .WithMessage("A target is required for session, speaker and venue feedback.");

        RuleFor(x => x.TargetType)
            .IsInEnum()
            .WithMessage("Invalid feedback target.");
    }
}
