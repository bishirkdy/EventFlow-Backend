using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionRegistrationInfo;

public sealed class GetSessionRegistrationInfoQueryHandler(ISessionRepository sessionRepository)
    : IRequestHandler<GetSessionRegistrationInfoQuery, GetSessionRegistrationInfoResponse>
{
    public async Task<GetSessionRegistrationInfoResponse> Handle(
        GetSessionRegistrationInfoQuery request,
        CancellationToken cancellationToken)
    {
        var session = await sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);

        if (session is null || session.EventId != request.EventId)
        {
            return new GetSessionRegistrationInfoResponse(
                false,
                request.EventId,
                request.SessionId,
                Guid.Empty,
                false,
                null,
                string.Empty);
        }

        return new GetSessionRegistrationInfoResponse(
            true,
            session.EventId,
            session.Id,
            session.SectionId,
            session.AllowRegistration,
            session.Capacity,
            session.Status);
    }
}
