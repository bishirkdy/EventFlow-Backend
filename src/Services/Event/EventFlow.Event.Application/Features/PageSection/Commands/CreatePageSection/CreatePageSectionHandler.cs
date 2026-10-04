using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Commands.CreatePageSection
{
    public sealed class CreatePageSectionHandler(
        IPageSectionRepository pageSectionRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<CreatePageSectionCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreatePageSectionCommand request,
            CancellationToken cancellationToken)
        {
            var existingSections = await pageSectionRepository.GetByPageIdAsync(
                request.PageId,
                includeUnpublished: true,
                cancellationToken);

            var displayOrder = existingSections.Count == 0
                ? 1
                : existingSections.Max(x => x.DisplayOrder) + 1;

            var section = new Domain.Entities.PageSection(
                request.PageId,
                request.SectionType,
                request.Title,
                request.Content,
                request.ImageUrl,
                request.ImagePublicId,
                displayOrder,
                request.Configuration);

            await pageSectionRepository.AddAsync(section, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return section.Id;
        }
    }
}
