using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationStats;

public sealed class GetRegistrationStatsQueryHandler(
    IRegistrationRepository registrations,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        GetRegistrationStatsQuery,
        RegistrationStatsDto>
{
    public async Task<RegistrationStatsDto> Handle(
        GetRegistrationStatsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var stats = await registrations.GetStatisticsAsync(
            query.EventId,
            cancellationToken);

        return new RegistrationStatsDto
        {
            Total = stats.Total,
            Pending = stats.Pending,
            Approved = stats.Approved,
            Rejected = stats.Rejected,
            Cancelled = stats.Cancelled,
            Waitlisted = stats.Waitlisted,
            Participants = stats.Participants,
            ActiveTickets = stats.ActiveTickets
        };
    }
}
