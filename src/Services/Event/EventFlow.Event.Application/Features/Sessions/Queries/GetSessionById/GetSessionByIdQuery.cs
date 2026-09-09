

using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionById
{
    public sealed record GetSessionByIdQuery(Guid Id,Guid EventId): IRequest<GetSessionByIdResponse?>;
}
