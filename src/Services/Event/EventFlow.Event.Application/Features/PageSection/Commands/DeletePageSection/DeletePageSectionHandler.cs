
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Commands.DeletePageSection
{
    public sealed class DeletePageSectionHandler(IPageSectionRepository pageSectionRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<DeletePageSectionCommand, string?>
    {
        public async Task<string?> Handle(DeletePageSectionCommand request, CancellationToken cancellationToken)
        {
            var section = await pageSectionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (section is null || section.PageId != request.PageId)
                throw new NotFoundException("Page section not found.");

            var imagePublicId = section.ImagePublicId;

            pageSectionRepository.Remove(section);

            var remainingSections = (await pageSectionRepository.GetByPageIdAsync(
                request.PageId,
                cancellationToken))
                .Where(x => x.Id != section.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            for (var i = 0; i < remainingSections.Count; i++)
                remainingSections[i].UpdateDisplayOrder(i + 1);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return imagePublicId;
        }
    }
}
