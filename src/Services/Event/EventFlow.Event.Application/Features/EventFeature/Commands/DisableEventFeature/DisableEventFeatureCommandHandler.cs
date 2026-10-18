

using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Commands.DisableEventFeature;

public sealed class DisableEventFeatureCommandHandler(IEventFeatureRepository eventFeatureRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DisableEventFeatureCommand>
{
    public async Task Handle(DisableEventFeatureCommand request, CancellationToken cancellationToken)
    {
        var eventFeature =
            await eventFeatureRepository.GetByEventAndFeatureAsync(request.EventId, request.FeatureId, cancellationToken);

        if (eventFeature is null)
            throw new NotFoundException("Event feature not found.");

        eventFeature.Disable();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
