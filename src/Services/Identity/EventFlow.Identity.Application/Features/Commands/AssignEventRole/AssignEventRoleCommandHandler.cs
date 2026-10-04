using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.AssignEventRole;

public sealed class AssignEventRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserEventRoleRepository userEventRoleRepository)
    : IRequestHandler<AssignEventRoleCommand, Guid>
{
    public async Task<Guid> Handle(
        AssignEventRoleCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var role = await roleRepository.GetByNameAsync(
            request.RoleName,
            cancellationToken);

        if (role is null)
        {
            throw new NotFoundException(
                $"Role '{request.RoleName}' not found.");
        }

        var existingRoles = await userEventRoleRepository.GetByUserAndEventAsync(
            request.UserId,
            request.EventId,
            cancellationToken);

        var existing = existingRoles.FirstOrDefault(
            x => x.RoleId == role.Id);

        if (existing is not null)
        {
            return existing.Id;
        }

        var userEventRole = new UserEventRole(
            request.UserId,
            request.EventId,
            role.Id);

        await userEventRoleRepository.AddAsync(
            userEventRole,
            cancellationToken);

        await userEventRoleRepository.SaveChangesAsync(cancellationToken);

        return userEventRole.Id;
    }
}
