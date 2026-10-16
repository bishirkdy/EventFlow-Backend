using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionById
{
    public sealed class GetSessionByIdQueryHandler(ISessionRepository sessionRepository, IMapper mapper, IEventFeatureRepository featureRepository)
        : IRequestHandler<GetSessionByIdQuery, GetSessionByIdResponse?>
    {
        public async Task<GetSessionByIdResponse?> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
        {
            await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Sessions, "sessions", cancellationToken);

            // Get session
            var session = await sessionRepository.GetByIdAsync(request.Id, cancellationToken);

            // Verify session belongs to event
            if (session is null || session.EventId != request.EventId)
                return null;

            // Map entity to response
            return mapper.Map<GetSessionByIdResponse>(session);
        }
    }
}
