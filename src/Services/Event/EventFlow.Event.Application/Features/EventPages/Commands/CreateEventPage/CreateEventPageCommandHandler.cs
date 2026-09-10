using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using EventFlow.Event.Domain.Entities;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.CreateEventPage
{
    public sealed class CreateEventPageCommandHandler(IEventPageRepository eventPageRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<CreateEventPageCommand, Guid>
    {
        public async Task<Guid> Handle(CreateEventPageCommand request, CancellationToken cancellationToken)
        {
            // Check duplicate page
            var existingPage = await eventPageRepository.GetBySlugAsync(request.EventId,request.Slug, cancellationToken);

            if (existingPage is not null)
                throw new ConflictException("A page with this slug already exists.");

            // Create page
            var page = new EventPage(
                request.EventId,
                request.Name,
                request.Slug,
                request.PageType,
                request.DisplayOrder);

            // Save page
            await eventPageRepository.AddAsync(page,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return page.Id;
        }
    }
}
