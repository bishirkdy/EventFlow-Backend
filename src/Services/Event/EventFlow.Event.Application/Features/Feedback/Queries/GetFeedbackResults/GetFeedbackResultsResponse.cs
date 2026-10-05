namespace EventFlow.Event.Application.Features.Feedback.Queries.GetFeedbackResults;

public sealed record GetFeedbackResultsResponse(
    int TotalCount,
    double AverageRating,
    IReadOnlyList<FeedbackRatingBucket> RatingDistribution,
    IReadOnlyList<FeedbackTargetSummary> Targets,
    IReadOnlyList<FeedbackItemResponse> RecentFeedback);
