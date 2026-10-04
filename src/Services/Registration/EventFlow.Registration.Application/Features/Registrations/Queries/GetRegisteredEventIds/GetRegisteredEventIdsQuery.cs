using EventFlow.Contracts.Common;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegisteredEventIds;

public sealed record GetRegisteredEventIdsQuery(
    Guid UserId)
    : IRequest<ApiResponse<IReadOnlyList<Guid>>>;
