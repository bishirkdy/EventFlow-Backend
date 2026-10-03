using EventFlow.SharedKernel.Exceptions;


using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Services;
using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Commands.EnableEventFeature
{
    public sealed class EnableEventFeatureCommandHandler(
        IEventFeatureRepository eventFeatureRepository,
        IEventRepository eventRepository,
        IEventTypeFeatureRepository eventTypeFeatureRepository,
        IFeatureRepository featureRepository,
        IEventWebsiteProvisioningService websiteProvisioning,
        IUnitOfWork unitOfWork)
        : IRequestHandler<EnableEventFeatureCommand>
    {
        public async Task Handle(EnableEventFeatureCommand request,CancellationToken cancellationToken)
        {
            var eventEntity = await eventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (eventEntity is null)
                throw new NotFoundException("Event not found.");

            var applicableFeatures = await eventTypeFeatureRepository.GetByEventTypeIdAsync(eventEntity.EventTypeId, cancellationToken);
            if (!applicableFeatures.Any(x => x.FeatureId == request.FeatureId))
                throw new NotFoundException("This feature is not available for this event type.");

            // Find existing event feature
            var eventFeature =
                await eventFeatureRepository.GetByEventAndFeatureAsync(
                    request.EventId,
                    request.FeatureId,
                    cancellationToken);

            if (eventFeature is null)
            {
                // Create event feature
                eventFeature = new Domain.Entities.EventFeature(request.EventId,request.FeatureId);

                await eventFeatureRepository.AddAsync(eventFeature,cancellationToken);
            }
            else
            {
                // Enable existing feature
                eventFeature.Enable();
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Turning a feature on also sets up its website page and section
            var feature = await featureRepository.GetByIdAsync(request.FeatureId, cancellationToken);

            if (feature is not null)
            {
                await websiteProvisioning.EnsureFeaturePageAsync(
                    request.EventId,
                    feature.Code,
                    cancellationToken);
            }
        }
    }
}
