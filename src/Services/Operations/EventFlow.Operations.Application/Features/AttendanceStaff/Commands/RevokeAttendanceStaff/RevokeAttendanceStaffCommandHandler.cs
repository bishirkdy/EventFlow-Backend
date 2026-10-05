using EventFlow.Operations.Application.Abstractions;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.AttendanceStaff.Commands.RevokeAttendanceStaff;

public sealed class RevokeAttendanceStaffCommandHandler(
    IOperationsDbContext db,
    IEventAuthorizationClient authorization,
    IIdentityClient identity,
    ICurrentUserService currentUser)
    : IRequestHandler<RevokeAttendanceStaffCommand, object?>
{
    private const string AttendanceStaffRoleName = "AttendanceStaff";

    public async Task<object?> Handle(
        RevokeAttendanceStaffCommand command,
        CancellationToken cancellationToken)
    {
        if (!await authorization.HasPermissionAsync(
                currentUser.UserId,
                command.EventId,
                "event.team.manage",
                cancellationToken))
        {
            throw new ForbiddenException(
                "You do not have permission to manage attendance staff.");
        }

        var entity = await db.AttendanceStaffAssignments.SingleOrDefaultAsync(
            x => x.Id == command.AssignmentId
                 && x.EventId == command.EventId
                 && x.IsActive,
            cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException("Attendance staff assignment not found.");
        }

        entity.IsActive = false;
        entity.RevokedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        // Drop the event role only when no other active assignment remains.
        var stillAssigned = await db.AttendanceStaffAssignments.AnyAsync(
            x => x.EventId == command.EventId
                 && x.UserId == entity.UserId
                 && x.IsActive,
            cancellationToken);

        if (!stillAssigned)
        {
            await identity.RemoveEventRoleAsync(
                entity.UserId,
                command.EventId,
                AttendanceStaffRoleName,
                cancellationToken);
        }

        return null;
    }
}
