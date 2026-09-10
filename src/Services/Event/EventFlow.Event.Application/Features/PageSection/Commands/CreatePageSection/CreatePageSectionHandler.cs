using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Commands.CreatePageSection
{
    public sealed class CreatePageSectionHandler(IPageSectionRepository pageSectionRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<CreatePageSectionCommand, Guid>
    {
        public async Task<Guid> Handle(CreatePageSectionCommand request,CancellationToken cancellationToken)
        {
            // Create page section
            var section = new Domain.Entities.PageSection(
                request.PageId,
                request.SectionType,
                request.Title,
                request.Content,
                request.ImageUrl,
                request.DisplayOrder,
                request.Configuration);

            await pageSectionRepository.AddAsync(section, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return section.Id;
        }
    }
}
