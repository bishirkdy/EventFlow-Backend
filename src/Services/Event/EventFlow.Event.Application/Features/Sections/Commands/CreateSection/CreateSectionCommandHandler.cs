using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Services;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Event.Application.Features.Sections.Commands.CreateSection;

public sealed class CreateSectionCommandHandler(ISectionRepository sectionRepository, IUnitOfWork unitOfWork, IEventWebsiteProvisioningService websiteProvisioningService, IEventFeatureRepository featureRepository)
    : IRequestHandler<CreateSectionCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateSectionCommand request,
        CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Sessions, "sessions", cancellationToken);

        var section = new Section(
            request.EventId,
            request.Name,
            request.Description,
            request.DisplayOrder);

        await sectionRepository.AddAsync(section, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await websiteProvisioningService.EnsureResourcePageAsync(request.EventId, "schedule", cancellationToken);

        return section.Id;
    }
}
