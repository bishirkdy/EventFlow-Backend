using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationStats;

public sealed record GetRegistrationStatsQuery(
    Guid EventId)
    : IRequest<ApiResponse<RegistrationStatsDto>>;
