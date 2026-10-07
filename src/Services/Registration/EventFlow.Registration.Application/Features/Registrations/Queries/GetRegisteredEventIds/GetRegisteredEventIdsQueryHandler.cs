namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegisteredEventIds;

public sealed class GetRegisteredEventIdsQueryHandler(
    IRegistrationRepository registrations)
    : IRequestHandler<GetRegisteredEventIdsQuery, IReadOnlyList<Guid>>
{
    public async Task<IReadOnlyList<Guid>> Handle(GetRegisteredEventIdsQuery query, CancellationToken cancellationToken)
    {
        var eventIds = await registrations.GetEventIdsForUserAsync(
            query.UserId,
            cancellationToken);

        return eventIds;
    }
}
