using EventFlow.Event.Domain.Enums;

namespace EventFlow.Event.Application.Features.Feedback.Queries.GetFeedbackResults;

public sealed record FeedbackItemResponse(
    Guid Id,
    FeedbackTargetType TargetType,
    Guid TargetId,
    string TargetName,
    int Rating,
    string? Comment,
    DateTime SubmittedAtUtc);
