using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Entities;
using EventFlow.Operations.Domain.Enums;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.AttendanceStaff.Commands.AssignAttendanceStaff;

public sealed class AssignAttendanceStaffCommandHandler(
    IOperationsDbContext db,
    IEventAuthorizationClient authorization,
    IIdentityClient identity,
    ICurrentUserService currentUser)
    : IRequestHandler<AssignAttendanceStaffCommand, AttendanceStaffDto>
{
    private const string AttendanceStaffRoleName = "AttendanceStaff";

    public async Task<AttendanceStaffDto> Handle(
        AssignAttendanceStaffCommand command,
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

        var user = await identity.FindByEmailAsync(command.Email, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException(
                "No user was found with that email address.");
        }

        var exists = await db.AttendanceStaffAssignments.FirstOrDefaultAsync(
            x => x.EventId == command.EventId
                 && x.UserId == user.Id
                 && x.ScopeType == command.ScopeType
                 && x.ScopeId == command.ScopeId
                 && x.IsActive,
            cancellationToken);

        if (exists is not null)
        {
            // Repair older assignments that predate event-role provisioning.
            await identity.AssignEventRoleAsync(
                user.Id,
                command.EventId,
                AttendanceStaffRoleName,
                cancellationToken);

            return ToDto(exists, user.Email);
        }

        var entity = new AttendanceStaffAssignment
        {
            EventId = command.EventId,
            UserId = user.Id,
            ScopeType = command.ScopeType,
            ScopeId = command.ScopeId
        };

        db.AttendanceStaffAssignments.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        // The assignee needs the AttendanceStaff event role so they can open
        // the staff portal (frontend guard + event.view checks).
        var roleAssigned = await identity.AssignEventRoleAsync(
            user.Id,
            command.EventId,
            AttendanceStaffRoleName,
            cancellationToken);

        if (!roleAssigned)
        {
            db.AttendanceStaffAssignments.Remove(entity);
            await db.SaveChangesAsync(cancellationToken);

            throw new ConflictException(
                "Attendance staff assignment failed because the event role could not be assigned.");
        }

        return ToDto(entity, user.Email);
    }

    private static AttendanceStaffDto ToDto(
        AttendanceStaffAssignment assignment,
        string email) => new(
        assignment.Id,
        assignment.EventId,
        assignment.UserId,
        email,
        assignment.ScopeType,
        assignment.ScopeId,
        assignment.IsActive);
}
