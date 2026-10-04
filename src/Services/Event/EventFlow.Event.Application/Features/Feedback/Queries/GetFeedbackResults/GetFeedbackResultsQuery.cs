using EventFlow.Event.Domain.Enums;
using MediatR;

namespace EventFlow.Event.Application.Features.Feedback.Queries.GetFeedbackResults;

public sealed record GetFeedbackResultsQuery(
    Guid EventId,
    Guid RequestedBy) : IRequest<GetFeedbackResultsResponse>;

public sealed record FeedbackRatingBucket(
    int Rating,
    int Count);

public sealed record FeedbackTargetSummary(
    FeedbackTargetType TargetType,
    Guid TargetId,
    string TargetName,
    int Count,
    double AverageRating);

public sealed record FeedbackItemResponse(
    Guid Id,
    FeedbackTargetType TargetType,
    Guid TargetId,
    string TargetName,
    int Rating,
    string? Comment,
    DateTime SubmittedAtUtc);

public sealed record GetFeedbackResultsResponse(
    int TotalCount,
    double AverageRating,
    IReadOnlyList<FeedbackRatingBucket> RatingDistribution,
    IReadOnlyList<FeedbackTargetSummary> Targets,
    IReadOnlyList<FeedbackItemResponse> RecentFeedback);
