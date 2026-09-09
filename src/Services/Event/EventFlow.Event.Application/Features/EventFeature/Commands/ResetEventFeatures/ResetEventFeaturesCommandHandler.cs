

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Commands.ResetEventFeatures
{
    public sealed class ResetEventFeaturesCommandHandler(IEventFeatureRepository eventFeatureRepository,IUnitOfWork unitOfWork) : IRequestHandler<ResetEventFeaturesCommand>
    {
        public async Task Handle(ResetEventFeaturesCommand request, CancellationToken cancellationToken)
        {
            // Get event features
            var features = await eventFeatureRepository.GetByEventIdAsync(request.EventId, cancellationToken);

            if (features.Count == 0)
                throw new NotFoundException("No event features found.");

            // Disable all features
            foreach (var feature in features)
            {
                feature.Disable();
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
