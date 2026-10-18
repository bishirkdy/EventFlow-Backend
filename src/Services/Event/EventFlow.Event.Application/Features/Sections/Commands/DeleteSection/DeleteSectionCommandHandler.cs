using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Sections.Commands.DeleteSection;

public sealed class DeleteSectionCommandHandler(ISectionRepository sectionRepository, IUnitOfWork unitOfWork, IEventFeatureRepository featureRepository)
    : IRequestHandler<DeleteSectionCommand>
{
    public async Task Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Sessions, "sessions", cancellationToken);

        var section = await sectionRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (section is null || section.EventId != request.EventId)
            throw new NotFoundException("Section not found.");

        section.Deactivate();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
