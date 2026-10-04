using EventFlow.Identity.Application.Abstractions.Repositories;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.RemoveEventRole;

public sealed class RemoveEventRoleCommandHandler(
    IRoleRepository roleRepository,
    IUserEventRoleRepository userEventRoleRepository)
    : IRequestHandler<RemoveEventRoleCommand>
{
    public async Task Handle(
        RemoveEventRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByNameAsync(
            request.RoleName,
            cancellationToken);

        if (role is null)
        {
            return;
        }

        var existingRoles = await userEventRoleRepository.GetByUserAndEventAsync(
            request.UserId,
            request.EventId,
            cancellationToken);

        var matches = existingRoles
            .Where(x => x.RoleId == role.Id)
            .ToList();

        if (matches.Count == 0)
        {
            return;
        }

        foreach (var match in matches)
        {
            await userEventRoleRepository.DeleteAsync(match);
        }

        await userEventRoleRepository.SaveChangesAsync(cancellationToken);
    }
}
