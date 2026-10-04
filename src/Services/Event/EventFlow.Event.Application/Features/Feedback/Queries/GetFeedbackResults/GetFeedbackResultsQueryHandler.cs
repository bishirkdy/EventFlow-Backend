using EventFlow.Event.Application.Abstractions.Authorization;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Domain.Enums;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Feedback.Queries.GetFeedbackResults;

public sealed class GetFeedbackResultsQueryHandler(
    IFeedbackRepository feedbackRepository,
    IEventRepository eventRepository,
    ISessionRepository sessionRepository,
    ISpeakerRepository speakerRepository,
    IVenueRepository venueRepository,
    IEventFeatureRepository featureRepository,
    IAuthorizationService authorizationService) : IRequestHandler<GetFeedbackResultsQuery, GetFeedbackResultsResponse>
{
    private const int RecentFeedbackLimit = 50;

    public async Task<GetFeedbackResultsResponse> Handle(
        GetFeedbackResultsQuery request,
        CancellationToken cancellationToken)
    {
        // Feedback results are organizer/owner only.
        var canViewResults = await authorizationService.HasPermissionAsync(
            request.RequestedBy,
            request.EventId,
            "event.update",
            cancellationToken);

        if (!canViewResults)
        {
            throw new ForbiddenException("You do not have permission to view feedback results.");
        }

        await EventFeatureGuard.EnsureEnabledAsync(
            featureRepository,
            request.EventId,
            FeatureIds.Feedback,
            "feedback",
            cancellationToken);

        var feedback = await feedbackRepository.GetByEventAsync(
            request.EventId,
            cancellationToken);

        var targetNames = await BuildTargetNamesAsync(request.EventId, cancellationToken);

        var totalCount = feedback.Count;
        var averageRating = totalCount == 0
            ? 0d
            : Math.Round(feedback.Average(x => x.Rating), 2);

        var distribution = Enumerable.Range(1, 5)
            .Select(rating => new FeedbackRatingBucket(
                rating,
                feedback.Count(x => x.Rating == rating)))
            .ToList();

        var targets = feedback
            .GroupBy(x => new { x.TargetType, x.TargetId })
            .Select(group => new FeedbackTargetSummary(
                group.Key.TargetType,
                group.Key.TargetId ?? request.EventId,
                ResolveTargetName(targetNames, group.Key.TargetType, group.Key.TargetId ?? request.EventId, request.EventId),
                group.Count(),
                Math.Round(group.Average(x => x.Rating), 2)))
            .OrderByDescending(x => x.Count)
            .ToList();

        var recent = feedback
            .Take(RecentFeedbackLimit)
            .Select(x => new FeedbackItemResponse(
                x.Id,
                x.TargetType,
                x.TargetId ?? request.EventId,
                ResolveTargetName(targetNames, x.TargetType, x.TargetId ?? request.EventId, request.EventId),
                x.Rating,
                x.Comment,
                x.SubmittedAtUtc))
            .ToList();

        return new GetFeedbackResultsResponse(
            totalCount,
            averageRating,
            distribution,
            targets,
            recent);
    }

    private async Task<Dictionary<(FeedbackTargetType, Guid), string>> BuildTargetNamesAsync(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var names = new Dictionary<(FeedbackTargetType, Guid), string>();

        var eventEntity = await eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (eventEntity is not null)
        {
            names[(FeedbackTargetType.Event, eventId)] = eventEntity.Name;
        }

        foreach (var session in await sessionRepository.GetByEventIdAsync(eventId, cancellationToken))
        {
            names[(FeedbackTargetType.Session, session.Id)] = session.Title;
        }

        foreach (var speaker in await speakerRepository.GetByEventIdAsync(eventId, cancellationToken))
        {
            names[(FeedbackTargetType.Speaker, speaker.Id)] = speaker.Name;
        }

        foreach (var venue in await venueRepository.GetByEventIdAsync(eventId, cancellationToken))
        {
            names[(FeedbackTargetType.Venue, venue.Id)] = venue.Name;
        }

        return names;
    }

    private static string ResolveTargetName(
        Dictionary<(FeedbackTargetType, Guid), string> names,
        FeedbackTargetType targetType,
        Guid targetId,
        Guid eventId)
    {
        if (names.TryGetValue((targetType, targetId), out var name))
        {
            return name;
        }

        return targetType switch
        {
            FeedbackTargetType.Event => "Event",
            FeedbackTargetType.Session => "Session",
            FeedbackTargetType.Speaker => "Speaker",
            FeedbackTargetType.Venue => "Venue",
            _ => "Unknown"
        };
    }
}
