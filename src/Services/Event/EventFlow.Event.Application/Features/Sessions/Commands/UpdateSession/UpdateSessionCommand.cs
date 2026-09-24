using EventFlow.Event.Application.Abstractions.Storage;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Commands.UpdateSession
{
    public sealed record UpdateSessionCommand(
        Guid Id, Guid EventId, string Title, string? Description, string SessionType, int? Capacity, DateTime? StartTime, DateTime? EndTime, Guid? VenueId, UploadedFile? Image) : IRequest;
}
