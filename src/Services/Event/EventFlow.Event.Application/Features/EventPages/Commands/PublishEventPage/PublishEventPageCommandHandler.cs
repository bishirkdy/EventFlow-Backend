using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.PublishEventPage;

public sealed class PublishEventPageCommandHandler(IEventPageRepository eventPageRepository, IUnitOfWork unitOfWork) : IRequestHandler<PublishEventPageCommand>
{
    public async Task Handle(PublishEventPageCommand request, CancellationToken cancellationToken)
    {
        var page = await eventPageRepository.GetByIdAsync(request.Id, cancellationToken);

        if (page is null || page.EventId != request.EventId)
            throw new NotFoundException("Event page not found.");

        page.Publish();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
