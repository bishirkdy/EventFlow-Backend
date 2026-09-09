

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Sections.Commands.UpdateSection
{
    public sealed class UpdateSectionCommandHandler(ISectionRepository sectionRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateSectionCommand>
    {
        public async Task Handle(UpdateSectionCommand request,CancellationToken cancellationToken)
        {
            // Get section belonging to the event
            var section = await sectionRepository.GetByIdAsync(
               request.Id,
               cancellationToken);

            if (section is null || section.EventId != request.EventId)
                throw new NotFoundException("Section not found.");

            // Update section
            section.Update(
                request.Name,
                request.Description,
                request.DisplayOrder);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
