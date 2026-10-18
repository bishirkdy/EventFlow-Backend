using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Commands.CancelEvent;

public sealed class CancelEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork) : IRequestHandler<CancelEventCommand>
{
    public async Task Handle(CancelEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await eventRepository.GetByIdAsync(request.Id, cancellationToken);

        if (eventEntity is null)
            throw new NotFoundException("Event not found.");

        eventEntity.Cancel();

        eventRepository.Update(eventEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
