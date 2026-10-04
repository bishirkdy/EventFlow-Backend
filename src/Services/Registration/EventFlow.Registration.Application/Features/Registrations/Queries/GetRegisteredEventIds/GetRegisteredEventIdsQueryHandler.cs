using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegisteredEventIds;

public sealed class GetRegisteredEventIdsQueryHandler(
    IRegistrationRepository registrations)
    : IRequestHandler<
        GetRegisteredEventIdsQuery,
        ApiResponse<IReadOnlyList<Guid>>>
{
    public async Task<ApiResponse<IReadOnlyList<Guid>>> Handle(
        GetRegisteredEventIdsQuery query,
        CancellationToken cancellationToken)
    {
        var eventIds = await registrations.GetEventIdsForUserAsync(
            query.UserId,
            cancellationToken);

        return ApiResponse<IReadOnlyList<Guid>>.Success(
            eventIds,
            "Registered event ids retrieved successfully.");
    }
}
