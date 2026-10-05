namespace EventFlow.Event.Application.Features.Feedback.Commands.SubmitFeedback;

public sealed record SubmitFeedbackResponse(
    Guid FeedbackId,
    int Rating,
    DateTime SubmittedAtUtc);
