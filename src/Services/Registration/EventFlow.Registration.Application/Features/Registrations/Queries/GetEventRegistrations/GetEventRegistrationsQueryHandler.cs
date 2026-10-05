using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetEventRegistrations;

public sealed class GetEventRegistrationsQueryHandler(
    IRegistrationRepository registrations,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        GetEventRegistrationsQuery,
        PaginatedResponse<RegistrationDto>>
{
    public async Task<PaginatedResponse<RegistrationDto>> Handle(
        GetEventRegistrationsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
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

        return new PaginatedResponse<RegistrationDto>
        {
            Items = items.Select(x => x.ToDto()).ToList(),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }
}
