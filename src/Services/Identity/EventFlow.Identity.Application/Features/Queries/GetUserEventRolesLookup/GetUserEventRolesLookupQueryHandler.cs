using EventFlow.Identity.Application.Abstractions.Repositories;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetUserEventRolesLookup;

public sealed class GetUserEventRolesLookupQueryHandler(IUserEventRoleRepository userEventRoles)
    : IRequestHandler<GetUserEventRolesLookupQuery, IReadOnlyList<UserEventRoleLookupResponse>>
{
    public async Task<IReadOnlyList<UserEventRoleLookupResponse>> Handle(
        GetUserEventRolesLookupQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await userEventRoles.GetByUserAsync(
            request.UserId,
            cancellationToken);

        return roles
            .GroupBy(x => x.EventId)
            .Select(group => new UserEventRoleLookupResponse(
                group.Key,
                group.Select(x => x.Role.Name).Distinct().ToArray()))
            .ToList();
    }
}
