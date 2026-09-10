

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Commands.UpdatePageSection
{
    public sealed class UpdatePageSectionHandler(IPageSectionRepository pageSectionRepository,
        IUnitOfWork unitOfWork): IRequestHandler<UpdatePageSectionCommand>
    {
        public async Task Handle(UpdatePageSectionCommand request,CancellationToken cancellationToken)
        {
            // Get section
            var section = await pageSectionRepository.GetByIdAsync(request.Id,cancellationToken);

            // Validate section belongs to page
            if (section is null || section.PageId != request.PageId)
                throw new NotFoundException("Page section not found.");

            // Update section
            section.Update(request.SectionType,request.Title,request.Content,request.ImageUrl,request.DisplayOrder,request.Configuration);
            section.SetVisibility(request.IsVisible);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
