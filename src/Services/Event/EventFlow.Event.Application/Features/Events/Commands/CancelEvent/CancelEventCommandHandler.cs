using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;


namespace EventFlow.Event.Application.Features.Events.Commands.CancelEvent
{
    // Command Handler
    public sealed class CancelEventCommandHandler(IEventRepository eventRepository,IUnitOfWork unitOfWork): IRequestHandler<CancelEventCommand>
    {
        public async Task Handle(CancelEventCommand request, CancellationToken cancellationToken)
        {
            // Get the event
            var eventEntity = await eventRepository.GetByIdAsync(request.Id, cancellationToken);

            // Check whether the event exists
            if (eventEntity is null)
                throw new KeyNotFoundException("Event not found.");

            // Apply the cancel business rule
            eventEntity.Cancel();

            eventRepository.Update(eventEntity);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
