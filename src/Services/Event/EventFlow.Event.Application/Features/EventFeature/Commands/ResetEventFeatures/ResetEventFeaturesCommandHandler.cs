using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Services;
using EventFlow.SharedKernel.Exceptions;
using EventFlow.Event.Application.Features.EventFeature.Queries.GetEventFeatures;
using EventFlow.Event.Domain.Entities;
using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Commands.ResetEventFeatures;

public sealed class ResetEventFeaturesCommandHandler(
    IEventRepository eventRepository,
    IEventFeatureRepository eventFeatureRepository,
    IEventTypeFeatureRepository eventTypeFeatureRepository,
    IEventWebsiteProvisioningService websiteProvisioning,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        ResetEventFeaturesCommand,
        IReadOnlyList<GetEventFeaturesResponse>>
{
    public async Task<IReadOnlyList<GetEventFeaturesResponse>> Handle(
        ResetEventFeaturesCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await eventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (eventEntity is null)
        {
            throw new NotFoundException("Event not found.");
        }

        var defaultFeatures = await eventTypeFeatureRepository.GetByEventTypeIdAsync(eventEntity.EventTypeId, cancellationToken);

        if (defaultFeatures.Count == 0)
        {
            throw new NotFoundException("No default features configured for this event type.");
        }

        var currentFeatures =
            await eventFeatureRepository.GetByEventIdAsync(request.EventId, cancellationToken);

        foreach (var feature in currentFeatures)
        {
            eventFeatureRepository.Remove(feature);
        }

        foreach (var defaultFeature in defaultFeatures)
        {
            var eventFeature = new Domain.Entities.EventFeature(
                request.EventId,
                defaultFeature.FeatureId,
                defaultFeature.IsEnabledByDefault);

            await eventFeatureRepository.AddAsync(eventFeature, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var resetFeatures = await eventFeatureRepository.GetByEventIdAsync(request.EventId, cancellationToken);

        // Re-create the website pages of every feature that is on by default
        foreach (var enabledFeature in resetFeatures.Where(x => x.IsEnabled))
        {
            await websiteProvisioning.EnsureFeaturePageAsync(
                request.EventId,
                enabledFeature.Feature.Code,
                cancellationToken);
        }

        return resetFeatures
            .Select(feature => new GetEventFeaturesResponse(
                feature.Id,
                feature.EventId,
                feature.FeatureId,
                feature.Feature.Code,
                feature.Feature.Name,
                feature.Feature.Description,
                feature.IsEnabled))
            .ToList();
    }
}