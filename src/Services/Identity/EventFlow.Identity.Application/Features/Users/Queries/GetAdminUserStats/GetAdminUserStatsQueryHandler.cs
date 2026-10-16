using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Application.DTOs.Admin;
using MediatR;

namespace EventFlow.Identity.Application.Features.Users.Queries.GetAdminUserStats;

public sealed class GetAdminUserStatsQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetAdminUserStatsQuery, AdminUserStatsResponse>
{
    public async Task<AdminUserStatsResponse> Handle(
        GetAdminUserStatsQuery request,
        CancellationToken cancellationToken)
    {
        var counts = await userRepository.GetCountsAsync(cancellationToken);

        return new AdminUserStatsResponse(
            counts.TotalUsers,
            counts.ActiveUsers,
            counts.PlatformAdmins);
    }
}
