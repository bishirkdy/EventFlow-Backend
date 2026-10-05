using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.ReorderEventPages
{
    public sealed class ReorderEventPagesCommandHandler(
        IEventPageRepository eventPageRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<ReorderEventPagesCommand>
    {
        public async Task Handle(
            ReorderEventPagesCommand request,
            CancellationToken cancellationToken)
        {
            var pages = await eventPageRepository.GetByEventIdAsync(
                request.EventId,
                cancellationToken);

            if (pages.Count != request.PageIds.Count ||
                request.PageIds.Any(id => pages.All(x => x.Id != id)))
            {
                throw new NotFoundException("One or more event pages were not found.");
            }

            for (var i = 0; i < request.PageIds.Count; i++)
            {
                var page = pages.First(x => x.Id == request.PageIds[i]);
                page.UpdateDisplayOrder(i + 1);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
