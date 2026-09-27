using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionsbyEvent
{
    public sealed class GetSessionsByEventQueryHandler(ISessionRepository sessionRepository )
        : IRequestHandler<GetSessionsByEventQuery, IReadOnlyList<GetSessionsByEventResponse>>
    {
        public async Task<IReadOnlyList<GetSessionsByEventResponse>> Handle(GetSessionsByEventQuery request, CancellationToken cancellationToken)
        {
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
           session.Status,
           session.CreatedAt,
           session.UpdatedAt))
        .ToList();
    }
    }
}
