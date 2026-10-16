using FluentValidation;

namespace EventFlow.Identity.Application.Features.Users.Queries.GetAdminUserStats;

public sealed class GetAdminUserStatsQueryValidator : AbstractValidator<GetAdminUserStatsQuery>
{
    public GetAdminUserStatsQueryValidator()
    {
    }
}
