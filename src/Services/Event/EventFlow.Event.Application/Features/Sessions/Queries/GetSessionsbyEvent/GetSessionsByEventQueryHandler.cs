using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionsbyEvent
{
    public sealed class GetSessionsByEventQueryHandler(ISessionRepository sessionRepository, IEventFeatureRepository featureRepository )
        : IRequestHandler<GetSessionsByEventQuery, IReadOnlyList<GetSessionsByEventResponse>>
    {
        public async Task<IReadOnlyList<GetSessionsByEventResponse>> Handle(GetSessionsByEventQuery request, CancellationToken cancellationToken)
        {
            await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Sessions, "sessions", cancellationToken);

            // Get sessions belonging to event
            var sessions = await sessionRepository.GetByEventIdAsync(request.EventId, cancellationToken);

      // Map entities to responses
      return sessions
        .Select(session => new GetSessionsByEventResponse(
           session.Id,
           session.EventId,
           session.SectionId,
           session.Title,
           session.Description,
           session.SessionType,
           session.Capacity,
           session.StartTimeUtc,
           session.EndTimeUtc,
           session.VenueId,
           session.ImageUrl,
           session.AllowRegistration,
           session.Status,
           session.CreatedAt,
           session.UpdatedAt))
        .ToList();
    }
    }
}
