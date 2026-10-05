using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.Attendance.Commands.CheckOut;

public sealed class CheckOutCommandHandler(
    IOperationsDbContext db,
    IEventAuthorizationClient authorization,
    ICurrentUserService currentUser)
    : IRequestHandler<CheckOutCommand, AttendanceDto>
{
    public async Task<AttendanceDto> Handle(
        CheckOutCommand command,
        CancellationToken cancellationToken)
    {
        var actorUserId = currentUser.UserId;

        var record = await db.AttendanceRecords.SingleOrDefaultAsync(
            x => x.Id == command.AttendanceId && x.EventId == command.EventId,
            cancellationToken);

        if (record is null)
        {
            throw new NotFoundException("Attendance record not found.");
        }

        if (!await AttendanceAuthorization.CanOperateAsync(
                db,
                authorization,
                command.EventId,
                actorUserId,
                record.SectionId,
                record.SessionId,
                cancellationToken))
        {
            throw new ForbiddenException("You are not assigned to this attendance scope.");
        }

        if (record.CheckedOutAtUtc is null)
        {
            record.CheckedOutAtUtc = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
        }

        return new AttendanceDto(
            record.Id,
            record.EventId,
            record.RegistrationId,
            record.ParticipantId,
            record.ParticipantUserId,
            record.SectionId,
            record.SessionId,
            record.StaffUserId,
            record.Method,
            record.CheckedInAtUtc,
            record.CheckedOutAtUtc);
    }
}
