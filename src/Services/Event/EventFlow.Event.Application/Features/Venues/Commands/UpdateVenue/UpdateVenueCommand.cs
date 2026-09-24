using EventFlow.Event.Application.Abstractions.Storage;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Commands.UpdateVenue
{
    public sealed record UpdateVenueCommand(
        Guid Id, Guid EventId, string Name, string? Description, string? Address, int Capacity, UploadedFile? Image) : IRequest;
}
