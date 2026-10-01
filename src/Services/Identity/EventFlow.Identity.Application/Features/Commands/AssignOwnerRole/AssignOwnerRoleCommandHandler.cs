using EventFlow.Identity.Application.Abstractions.Repositories;
using EventFlow.Identity.Domain.Entities;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Identity.Application.Features.Commands.AssignOwnerRole;

public sealed class AssignOwnerRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserEventRoleRepository userEventRoleRepository)
    : IRequestHandler<AssignOwnerRoleCommand, Guid>
{
    public async Task<Guid> Handle(
        AssignOwnerRoleCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("Owner user not found.");
        }

        var ownerRole = await roleRepository.GetOrCreateOwnerRoleAsync(cancellationToken);

        var existingRoles = await userEventRoleRepository.GetByUserAndEventAsync(
            request.UserId,
            request.EventId,
            cancellationToken);

        var existingOwner = existingRoles.FirstOrDefault(x => x.RoleId == ownerRole.Id);

        if (existingOwner is not null)
        {
            return existingOwner.Id;
        }

        var userEventRole = new UserEventRole(
            request.UserId,
            request.EventId,
            ownerRole.Id);

        await userEventRoleRepository.AddAsync(userEventRole, cancellationToken);
        await userEventRoleRepository.SaveChangesAsync(cancellationToken);

        return userEventRole.Id;
    }
}
