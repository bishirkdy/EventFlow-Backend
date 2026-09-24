using EventFlow.Event.Application.Abstractions.Storage;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Commands.CreateSession
{
    public sealed record CreateSessionCommand(
        Guid EventId, Guid SectionId, string Title, string? Description, string SessionType, int? Capacity, DateTime? StartTime, DateTime? EndTime, Guid? VenueId, UploadedFile? Image) : IRequest<Guid>;
}
