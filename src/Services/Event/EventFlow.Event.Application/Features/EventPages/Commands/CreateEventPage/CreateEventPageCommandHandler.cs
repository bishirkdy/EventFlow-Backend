using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using EventFlow.Event.Domain.Entities;
using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Commands.CreateEventPage
{
    public sealed class CreateEventPageCommandHandler(
        IEventPageRepository eventPageRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<CreateEventPageCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreateEventPageCommand request,
            CancellationToken cancellationToken)
        {
            var existingPage = await eventPageRepository.GetBySlugAsync(
                request.EventId,
                request.Slug,
                cancellationToken);

            if (existingPage is not null)
                throw new ConflictException("A page with this slug already exists.");

            var existingPages = await eventPageRepository.GetByEventIdAsync(
                request.EventId,
                cancellationToken);

            var displayOrder = existingPages.Count == 0
                ? 1
                : existingPages.Max(x => x.DisplayOrder) + 1;

            var page = new EventPage(
                request.EventId,
                request.Name,
                request.Slug,
                request.PageType,
                displayOrder);

            await eventPageRepository.AddAsync(page, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return page.Id;
        }
    }
}
