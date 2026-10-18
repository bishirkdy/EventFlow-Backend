using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionRegistrationInfo;

public sealed record GetSessionRegistrationInfoQuery(Guid EventId, Guid SessionId)
    : IRequest<GetSessionRegistrationInfoResponse>;
