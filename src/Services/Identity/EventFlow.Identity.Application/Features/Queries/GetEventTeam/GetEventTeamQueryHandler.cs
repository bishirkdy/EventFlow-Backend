using EventFlow.Identity.Application.Abstractions.Authorization;
using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Security.Authentication;
using EventFlow.Security.Authorization;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetEventTeam;

public sealed class GetEventTeamQueryHandler(
    IUserEventRoleRepository userEventRoleRepository,
    IPermissionService permissions,
    ICurrentUserService currentUser)
    : IRequestHandler<GetEventTeamQuery, IReadOnlyList<EventTeamMemberResponse>>
{
    public async Task<IReadOnlyList<EventTeamMemberResponse>> Handle(
        GetEventTeamQuery request,
        CancellationToken cancellationToken)
    {
        if (!await permissions.HasPermissionAsync(
                currentUser.UserId,
                request.EventId,
                PermissionConstants.Event.TeamManage,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission to manage the event team.");
        }

        var assignments = await userEventRoleRepository.GetByEventAsync(
            request.EventId,
            cancellationToken);

        return assignments
            .GroupBy(x => new
            {
                x.UserId,
                x.User.Email,
                x.User.UserName,
                x.User.FirstName,
                x.User.LastName
            })
            .Select(group => new EventTeamMemberResponse(
                group.Key.UserId,
                group.Key.Email,
                group.Key.UserName,
                group.Key.FirstName,
                group.Key.LastName,
                group.Select(x => new EventTeamRoleResponse(x.RoleId, x.Role.Name)).ToList()))
            .ToList();
    }
}
