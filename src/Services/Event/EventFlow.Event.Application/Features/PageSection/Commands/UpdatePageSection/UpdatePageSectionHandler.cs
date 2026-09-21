
    using EventFlow.Event.Application.Abstractions.Persistence;
    using EventFlow.Event.Application.Exceptions;
    using MediatR;

    namespace EventFlow.Event.Application.Features.PageSection.Commands.UpdatePageSection
    {
        public sealed class UpdatePageSectionHandler(IPageSectionRepository pageSectionRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UpdatePageSectionCommand , string?>
        {
            public async Task<string?> Handle(UpdatePageSectionCommand request,CancellationToken cancellationToken)
            {
                // Get section
                var section = await pageSectionRepository.GetByIdAsync(request.Id,cancellationToken);

                // Validate section belongs to page
                if (section is null || section.PageId != request.PageId)
                    throw new NotFoundException("Page section not found.");

                // Update section
                var oldImagePublicId = section.ImagePublicId;

                var imageUrl = request.ImageUrl ?? section.ImageUrl;
                var imagePublicId = request.ImagePublicId ?? section.ImagePublicId;

                section.Update(
                    request.SectionType,
                    request.Title,
                    request.Content,
                    imageUrl,
                    imagePublicId,
                    request.DisplayOrder,
                    request.Configuration);
                section.SetVisibility(request.IsVisible);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                // Only return the old image ID when a new image replaced it.
                if (request.ImagePublicId is not null)
                    return oldImagePublicId;

                return null;

            }
        }
    }
