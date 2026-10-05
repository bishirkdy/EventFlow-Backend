using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Entities;
using EventFlow.Operations.Domain.Enums;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Operations.Application.Features.Attendance.Commands.CheckInManual;

public sealed class CheckInManualCommandHandler(
    IOperationsDbContext db,
    IEventAuthorizationClient authorization,
    ICurrentUserService currentUser)
    : IRequestHandler<CheckInManualCommand, AttendanceDto>
{
    public async Task<AttendanceDto> Handle(
        CheckInManualCommand command,
        CancellationToken cancellationToken)
    {
        var actorUserId = currentUser.UserId;

        if (!await AttendanceAuthorization.CanOperateAsync(
                db,
                authorization,
                command.EventId,
                actorUserId,
                command.SectionId,
                command.SessionId,
                cancellationToken))
        {
            throw new ForbiddenException("You are not assigned to this attendance scope.");
        }

        var entity = new AttendanceRecord
        {
            EventId = command.EventId,
            RegistrationId = command.RegistrationId,
            ParticipantId = Guid.Empty,
            ParticipantUserId = Guid.Empty,
            SectionId = command.SectionId,
            SessionId = command.SessionId,
            StaffUserId = actorUserId,
            Method = AttendanceMethod.Manual,
            CheckedInAtUtc = DateTime.UtcNow
        };

        db.AttendanceRecords.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new AttendanceDto(
            entity.Id,
            entity.EventId,
            entity.RegistrationId,
            entity.ParticipantId,
            entity.ParticipantUserId,
            entity.SectionId,
            entity.SessionId,
            entity.StaffUserId,
            entity.Method,
            entity.CheckedInAtUtc,
            entity.CheckedOutAtUtc);
    }
}
