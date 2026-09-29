using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetEventRegistrations;

public sealed class GetEventRegistrationsQueryHandler(
    IRegistrationRepository registrations,
    ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        GetEventRegistrationsQuery,
        ApiResponse<PaginatedResponse<RegistrationDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<RegistrationDto>>> Handle(
        GetEventRegistrationsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<PaginatedResponse<RegistrationDto>>.Fail(
                ["You do not have permission."]);
        }

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var (items, totalCount) =
            await registrations.GetPagedForEventAsync(
                query.EventId,
                query.Status,
                query.Search,
                page,
                pageSize,
                cancellationToken);

        return ApiResponse<PaginatedResponse<RegistrationDto>>.Success(
            new PaginatedResponse<RegistrationDto>
            {
                Items = items.Select(x => x.ToDto()).ToList(),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            });
    }
}
