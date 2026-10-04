using EventFlow.Event.Domain.Enums;

namespace EventFlow.Event.Api.Requests.Feedback
{
    public sealed record SubmitFeedbackRequest(
        FeedbackTargetType TargetType,
        Guid? TargetId,
        int Rating,
        string? Comment = null);
}
