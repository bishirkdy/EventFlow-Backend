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
                throw new KeyNotFoundException("Event not found.");

            eventEntity.Update(
                request.Name,
                request.Description,
                request.EventType,
                request.SubType,
                request.StartDate,
                request.EndDate,
                request.TimeZone);

            eventRepository.Update(eventEntity);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
