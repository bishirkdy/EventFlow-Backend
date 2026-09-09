

using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Commands.EnableEventFeature
{
    public sealed class EnableEventFeatureCommandHandler(IEventFeatureRepository eventFeatureRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<EnableEventFeatureCommand>
    {
        public async Task Handle(EnableEventFeatureCommand request,CancellationToken cancellationToken)
        {
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
        }
    }
}
