
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Commands.DeletePageSection
{
    public sealed class DeletePageSectionHandler(
        IPageSectionRepository pageSectionRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<DeletePageSectionCommand>
    {
        public async Task Handle(DeletePageSectionCommand request,CancellationToken cancellationToken)
        {
            // Get section
            var section = await pageSectionRepository.GetByIdAsync(request.Id,cancellationToken);

            // Validate section belongs to page
            if (section is null || section.PageId != request.PageId)
                throw new NotFoundException("Page section not found.");

            // Delete section
            pageSectionRepository.Remove(section);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
