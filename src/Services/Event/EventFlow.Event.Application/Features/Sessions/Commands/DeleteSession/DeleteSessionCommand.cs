
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Commands.DeleteSession
{
    public sealed record DeleteSessionCommand(Guid Id, Guid EventId) : IRequest;
}
