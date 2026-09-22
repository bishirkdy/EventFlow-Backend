using EventFlow.Event.Application.Exceptions;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Commands.UpdateEvent;

public sealed class UpdateEventCommandHandler(
    IEventRepository eventRepository,
    IEventTypeRepository eventTypeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEventCommand>
{
    public async Task Handle(
        UpdateEventCommand request,
        CancellationToken cancellationToken)
    {
        var eventEntity = await eventRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (eventEntity is null)
            throw new NotFoundException("Event not found.");

        var eventType = await eventTypeRepository.GetByIdAsync(
            request.EventTypeId,
            cancellationToken);

        if (eventType is null || !eventType.IsActive)
            throw new NotFoundException("Event type not found.");

        var timeZone = TimeZoneHelper.GetTimeZone(request.TimeZone);

        var startDate = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(
                request.StartDate,
                DateTimeKind.Unspecified),
            timeZone);

        var endDate = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(
                request.EndDate,
                DateTimeKind.Unspecified),
            timeZone);

        eventEntity.Update(
            request.Name,
            request.Description,
            request.EventTypeId,
            request.SubType,
            startDate,
            endDate,
            request.TimeZone);

        eventRepository.Update(eventEntity);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}