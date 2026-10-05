using EventFlow.Event.Domain.Enums;
using MediatR;

namespace EventFlow.Event.Application.Features.Feedback.Commands.SubmitFeedback;

public sealed record SubmitFeedbackCommand(
    Guid EventId,
    Guid ParticipantUserId,
    FeedbackTargetType TargetType,
    Guid? TargetId,
    int Rating,
    string? Comment) : IRequest<SubmitFeedbackResponse>;
