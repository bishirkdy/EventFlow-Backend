using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetRegistrationFeature;

public sealed class GetRegistrationFeatureQueryHandler(
    IFeatureRepository features,
    IEventFeatureRepository eventFeatures)
    : IRequestHandler<GetRegistrationFeatureQuery, bool>
{
    public async Task<bool> Handle(
        GetRegistrationFeatureQuery request,
        CancellationToken cancellationToken)
    {
        var feature = await features.GetByCodeAsync("registration", cancellationToken);

        if (feature is null)
        {
            return false;
        }

        var eventFeature = await eventFeatures.GetByEventAndFeatureAsync(
            request.EventId,
            feature.Id,
            cancellationToken);

        return eventFeature?.IsEnabled == true;
    }
}
