using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionById
{
    public sealed class GetSessionByIdQueryHandler(ISessionRepository sessionRepository, IMapper mapper)
        : IRequestHandler<GetSessionByIdQuery, GetSessionByIdResponse?>
    {
        public async Task<GetSessionByIdResponse?> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
        {
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
