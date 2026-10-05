
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.UnpublishEventPage
{
    public sealed class UnpublishEventPageCommandHandler(IEventPageRepository eventPageRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<UnpublishEventPageCommand>
    {
        public async Task Handle(UnpublishEventPageCommand request, CancellationToken cancellationToken)
        {
            // Get page
            var page = await eventPageRepository.GetByIdAsync(request.Id,cancellationToken);

            if (page is null || page.EventId != request.EventId)
                throw new NotFoundException("Event page not found.");

            // Unpublish page
            page.Unpublish();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
