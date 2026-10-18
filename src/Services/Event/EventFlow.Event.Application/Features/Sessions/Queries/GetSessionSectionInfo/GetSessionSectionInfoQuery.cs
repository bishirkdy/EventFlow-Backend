using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionSectionInfo;

public sealed record GetSessionSectionInfoQuery(Guid EventId, Guid SessionId)
    : IRequest<GetSessionSectionInfoResponse>;
