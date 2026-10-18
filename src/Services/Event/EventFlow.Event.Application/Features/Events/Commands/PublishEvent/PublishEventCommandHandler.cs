using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Commands.PublishEvent;

public sealed class PublishEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork) : IRequestHandler<PublishEventCommand>
{
    public async Task Handle(PublishEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await eventRepository.GetByIdAsync(request.Id, cancellationToken);

        if (eventEntity is null)
            throw new NotFoundException("Event not found.");

        eventEntity.Publish();

        eventRepository.Update(eventEntity);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
