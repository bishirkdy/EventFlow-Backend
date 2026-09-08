using MediatR;


namespace EventFlow.Event.Application.Features.EventSettings.Queries.GetEventSettings
{
    // Query
    public sealed record GetEventSettingsQuery(Guid EventId): IRequest<GetEventSettingsResponse?>;
}
