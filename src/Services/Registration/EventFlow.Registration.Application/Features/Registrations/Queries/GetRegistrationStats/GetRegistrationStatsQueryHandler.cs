using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationStats;

public sealed class GetRegistrationStatsQueryHandler(
    IRegistrationRepository registrations,
    ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        GetRegistrationStatsQuery,
        ApiResponse<RegistrationStatsDto>>
{
    public async Task<ApiResponse<RegistrationStatsDto>> Handle(
        GetRegistrationStatsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<RegistrationStatsDto>.Fail(
                ["You do not have permission."]);
        }

        var stats = await registrations.GetStatisticsAsync(
            query.EventId,
            cancellationToken);

        return ApiResponse<RegistrationStatsDto>.Success(
            new RegistrationStatsDto
            {
                Total = stats.Total,
                Pending = stats.Pending,
                Approved = stats.Approved,
                Rejected = stats.Rejected,
                Cancelled = stats.Cancelled,
                Waitlisted = stats.Waitlisted,
                Participants = stats.Participants,
                ActiveTickets = stats.ActiveTickets
            });
    }
}
