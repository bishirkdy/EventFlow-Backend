using AutoMapper;
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
    ICurrentUserService currentUser,
    IMapper mapper)
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

        // Manual check-in is performed by staff on behalf of a participant.
        var entity = new AttendanceRecord(
            command.EventId,
            command.RegistrationId,
            Guid.Empty,
            Guid.Empty,
            command.SectionId,
            command.SessionId,
            actorUserId,
            AttendanceMethod.Manual);

        db.AttendanceRecords.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<AttendanceDto>(entity);
    }
}
