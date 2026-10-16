using FluentValidation;

namespace EventFlow.Event.Application.Features.Admin.Queries.GetAdminEventStats;

public sealed class GetAdminEventStatsQueryValidator : AbstractValidator<GetAdminEventStatsQuery>
{
    public GetAdminEventStatsQueryValidator()
    {
    }
}
