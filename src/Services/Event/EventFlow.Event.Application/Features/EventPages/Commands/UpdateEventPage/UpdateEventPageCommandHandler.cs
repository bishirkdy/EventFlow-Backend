using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.UpdateEventPage
{
    public sealed class UpdateEventPageCommandHandler(IEventPageRepository eventPageRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateEventPageCommand>
    {
        public async Task Handle(UpdateEventPageCommand request, CancellationToken cancellationToken)
        {
            var page = await eventPageRepository.GetByIdAsync(request.Id, cancellationToken);

            if (page is null || page.EventId != request.EventId)
            {
                throw new NotFoundException("Event page not found.");
            }

            page.Update(request.Name,request.Slug,request.PageType, request.DisplayOrder);
            eventPageRepository.Update(page);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}