using EventFlow.Event.Application.Exceptions;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;


namespace EventFlow.Event.Application.Features.Events.Commands.UpdateEvent
{
    //Handler for update event
    public sealed class UpdateEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateEventCommand>
    {
        public async Task Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await eventRepository.GetByIdAsync(request.Id, cancellationToken);

            if (eventEntity is null)
                throw new NotFoundException("Event not found.");

            var timeZone = TimeZoneHelper.GetTimeZone(request.TimeZone);

            var startDate = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(request.StartDate, DateTimeKind.Unspecified),
                timeZone);

            var endDate = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(request.EndDate, DateTimeKind.Unspecified),
                timeZone);

            eventEntity.Update(
                request.Name,
                request.Description,
                request.EventType,
                request.SubType,
                startDate,
                endDate,
                request.TimeZone);

            eventRepository.Update(eventEntity);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
