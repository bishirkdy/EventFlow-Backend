using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionSectionInfo
{
    public sealed class GetSessionSectionInfoQueryHandler(ISessionRepository sessionRepository)
        : IRequestHandler<GetSessionSectionInfoQuery, GetSessionSectionInfoResponse>
    {
        public async Task<GetSessionSectionInfoResponse> Handle(
            GetSessionSectionInfoQuery request,
            CancellationToken cancellationToken)
        {
            var session = await sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);

            if (session is null || session.EventId != request.EventId)
            {
                return new GetSessionSectionInfoResponse(false, Guid.Empty);
            }

            return new GetSessionSectionInfoResponse(true, session.SectionId);
        }
    }
}
