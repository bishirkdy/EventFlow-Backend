namespace EventFlow.Event.Application.Features.Feedback.Queries.GetFeedbackResults;

public sealed record FeedbackRatingBucket(
    int Rating,
    int Count);
