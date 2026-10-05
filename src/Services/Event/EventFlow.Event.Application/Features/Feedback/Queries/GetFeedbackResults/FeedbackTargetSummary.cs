using EventFlow.Event.Domain.Enums;

namespace EventFlow.Event.Application.Features.Feedback.Queries.GetFeedbackResults;

public sealed record FeedbackTargetSummary(
    FeedbackTargetType TargetType,
    Guid TargetId,
    string TargetName,
    int Count,
    double AverageRating);
