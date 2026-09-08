using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;


namespace EventFlow.Event.Application.Features.Events.Commands.PublishEvent
{
    // Command Handler
    public sealed class PublishEventCommandHandler(IEventRepository eventRepository,IUnitOfWork unitOfWork) : IRequestHandler<PublishEventCommand>
    {
        public async Task Handle(PublishEventCommand request,CancellationToken cancellationToken)
        {
            // Get the event
            var eventEntity = await eventRepository.GetByIdAsync(request.Id,cancellationToken);

            // Check whether the event exists
            if (eventEntity is null)
                throw new KeyNotFoundException("Event not found.");

            // Apply the publish business rule
            eventEntity.Publish();

            // Mark the entity as updated
            eventRepository.Update(eventEntity);

            // Save changes to the database
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
