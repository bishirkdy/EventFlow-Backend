using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionsbyEvent
{
    public sealed class GetSessionsByEventQueryHandler(ISessionRepository sessionRepository, IMapper mapper)
        : IRequestHandler<GetSessionsByEventQuery, IReadOnlyList<GetSessionsByEventResponse>>
    {
        public async Task<IReadOnlyList<GetSessionsByEventResponse>> Handle(GetSessionsByEventQuery request, CancellationToken cancellationToken)
        {
            // Get sessions belonging to event
            var sessions = await sessionRepository.GetByEventIdAsync(request.EventId, cancellationToken);

            // Map entities to responses
            return mapper.Map<IReadOnlyList<GetSessionsByEventResponse>>(sessions);
        }
    }
}
