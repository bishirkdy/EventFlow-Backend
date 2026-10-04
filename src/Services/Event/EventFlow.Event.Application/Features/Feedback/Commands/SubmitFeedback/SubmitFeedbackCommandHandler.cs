using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Domain.Enums;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using FeedbackEntity = EventFlow.Event.Domain.Entities.Feedback;

namespace EventFlow.Event.Application.Features.Feedback.Commands.SubmitFeedback;

public sealed class SubmitFeedbackCommandHandler(
    IFeedbackRepository feedbackRepository,
    IEventFeatureRepository featureRepository,
    ISessionRepository sessionRepository,
    ISpeakerRepository speakerRepository,
    IVenueRepository venueRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SubmitFeedbackCommand, SubmitFeedbackResponse>
{
    public async Task<SubmitFeedbackResponse> Handle(
        SubmitFeedbackCommand request,
        CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(
            featureRepository,
            request.EventId,
            FeatureIds.Feedback,
            "feedback",
            cancellationToken);

        // Event-level feedback stores the event id so the unique index
        // still enforces one entry per participant.
        var targetId = request.TargetType == FeedbackTargetType.Event
            ? request.EventId
            : request.TargetId;

        if (targetId is null || targetId == Guid.Empty)
        {
            throw new InvalidOperationException("A target is required for this feedback.");
        }

        await EnsureTargetExistsAsync(request.EventId, request.TargetType, targetId.Value, cancellationToken);

        var existing = await feedbackRepository.FindAsync(
            request.EventId,
            request.ParticipantUserId,
            request.TargetType,
            targetId,
            cancellationToken);

        if (existing is not null)
        {
            throw new ConflictException("You have already submitted feedback for this target.");
        }

        var feedback = new FeedbackEntity(
            request.EventId,
            request.ParticipantUserId,
            request.TargetType,
            targetId,
            request.Rating,
            request.Comment?.Trim());

        await feedbackRepository.AddAsync(feedback, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubmitFeedbackResponse(feedback.Id, feedback.Rating, feedback.SubmittedAtUtc);
    }

    private async Task EnsureTargetExistsAsync(
        Guid eventId,
        FeedbackTargetType targetType,
        Guid targetId,
        CancellationToken cancellationToken)
    {
        switch (targetType)
        {
            case FeedbackTargetType.Event:
                // Target id was normalized to the event id above.
                break;

            case FeedbackTargetType.Session:
            {
                var session = await sessionRepository.GetByIdAsync(targetId, cancellationToken);
                if (session is null || session.EventId != eventId)
                {
                    throw new NotFoundException("Session not found.");
                }

                break;
            }

            case FeedbackTargetType.Speaker:
            {
                var speaker = await speakerRepository.GetByIdAsync(targetId, cancellationToken);
                if (speaker is null || speaker.EventId != eventId)
                {
                    throw new NotFoundException("Speaker not found.");
                }

                break;
            }

            case FeedbackTargetType.Venue:
            {
                var venue = await venueRepository.GetByIdAsync(targetId, cancellationToken);
                if (venue is null || venue.EventId != eventId)
                {
                    throw new NotFoundException("Venue not found.");
                }

                break;
            }

            default:
                throw new InvalidOperationException("Invalid feedback target.");
        }
    }
}
