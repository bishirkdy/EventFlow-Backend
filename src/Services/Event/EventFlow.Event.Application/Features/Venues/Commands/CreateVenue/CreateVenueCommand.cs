using EventFlow.Event.Application.Abstractions.Storage;
using MediatR;

namespace EventFlow.Event.Application.Features.Venues.Commands.CreateVenue
{
    public sealed record CreateVenueCommand(
        Guid EventId, string Name, string? Description, string? Address, int Capacity, UploadedFile? Image) : IRequest<Guid>;
}
