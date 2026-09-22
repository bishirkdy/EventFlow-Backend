using EventFlow.Event.Application.Abstractions.Storage;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Commands.CreateEvent
{
    public sealed record CreateEventCommand(
        string Name,
        string? Description,
        Guid EventTypeId,
        string? SubType,
        DateTime StartDate,
        DateTime EndDate,
        string TimeZone,
        IReadOnlyList<UploadedFile> Images
    ) : IRequest<CreateEventResult>;
}
