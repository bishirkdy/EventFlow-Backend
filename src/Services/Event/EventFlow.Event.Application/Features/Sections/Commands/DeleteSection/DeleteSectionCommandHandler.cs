using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;


namespace EventFlow.Event.Application.Features.Sections.Commands.DeleteSection
{
    public sealed class DeleteSectionCommandHandler(ISectionRepository sectionRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteSectionCommand>
    {
        public async Task Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            // Get section belonging to the event
            var section = await sectionRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (section is null || section.EventId != request.EventId)
                throw new NotFoundException("Section not found.");

            // Deactivate section
            section.Deactivate();

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
