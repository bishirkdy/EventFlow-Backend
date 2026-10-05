using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Entities;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.AttendanceStaff.Queries.GetAttendanceStaff;

public sealed class GetAttendanceStaffQueryHandler(
    IOperationsDbContext db,
    IEventAuthorizationClient authorization,
    IIdentityClient identity,
    ICurrentUserService currentUser)
    : IRequestHandler<GetAttendanceStaffQuery, IReadOnlyList<AttendanceStaffDto>>
{
    public async Task<IReadOnlyList<AttendanceStaffDto>> Handle(
        GetAttendanceStaffQuery query,
        CancellationToken cancellationToken)
    {
        if (!await authorization.HasPermissionAsync(
                currentUser.UserId,
                query.EventId,
                "event.view",
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission to view attendance staff.");
        }

        var items = await db.AttendanceStaffAssignments
            .Where(x => x.EventId == query.EventId && x.IsActive)
            .OrderBy(x => x.ScopeType)
            .ToListAsync(cancellationToken);

        var result = new List<AttendanceStaffDto>(items.Count);
        foreach (var item in items)
        {
            var user = await identity.FindByIdAsync(item.UserId, cancellationToken);
            result.Add(ToDto(item, user?.Email ?? "Unknown user"));
        }

        return result;
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
