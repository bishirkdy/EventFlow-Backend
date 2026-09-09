

using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Commands.CreateSession
{
    public sealed record CreateSessionCommand(
        Guid EventId,
        Guid SectionId,
        string Title,
        string? Description,
        string SessionType,
        int? Capacity) : IRequest<Guid>;
}
