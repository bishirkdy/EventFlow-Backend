

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Commands.ReorderPageSections
{
    public sealed class ReorderPageSectionsHandler(IPageSectionRepository pageSectionRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<ReorderPageSectionsCommand>
    {
        public async Task Handle(ReorderPageSectionsCommand request, CancellationToken cancellationToken)
        {
            // Get all sections belonging to the page
            var sections = await pageSectionRepository.GetByPageIdAsync(request.PageId,cancellationToken);

            // Validate all section IDs belong to this page
            if (sections.Count != request.SectionIds.Count || request.SectionIds.Any(id => sections.All(x => x.Id != id)))
            {
                throw new NotFoundException("One or more page sections were not found.");
            }

            // Update display order
            for (var i = 0; i < request.SectionIds.Count; i++)
            {
                var section = sections.First(x => x.Id == request.SectionIds[i]);
                section.UpdateDisplayOrder(i + 1);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
