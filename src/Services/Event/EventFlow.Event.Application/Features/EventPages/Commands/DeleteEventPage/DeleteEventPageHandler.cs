

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.DeleteEventPage
{
    public sealed class DeleteEventPageHandler(IEventPageRepository eventPageRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteEventPageCommand>
    {
        public async Task Handle(DeleteEventPageCommand request, CancellationToken cancellationToken)
        {
            // Get page
            var page = await eventPageRepository.GetByIdAsync(request.Id, cancellationToken);

            // Validate page belongs to event
            if (page is null || page.EventId != request.EventId)
                throw new NotFoundException("Event page not found.");

            // Delete page
            eventPageRepository.Remove(page);

            var remainingPages = (await eventPageRepository.GetByEventIdAsync(
                request.EventId,
                cancellationToken))
                .Where(x => x.Id != page.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            for (var i = 0; i < remainingPages.Count; i++)
                remainingPages[i].UpdateDisplayOrder(i + 1);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
