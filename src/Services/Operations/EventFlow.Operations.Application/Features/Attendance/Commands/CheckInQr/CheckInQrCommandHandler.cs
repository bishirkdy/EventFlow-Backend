using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Application.Contracts;
using EventFlow.Operations.Domain.Entities;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Operations.Application.Features.Attendance.Commands.CheckInQr;

public sealed class CheckInQrCommandHandler(
    IOperationsDbContext db,
    IEventAuthorizationClient authorization,
    IRegistrationClient registrationClient,
    ICurrentUserService currentUser)
    : IRequestHandler<CheckInQrCommand, AttendanceDto>
{
    public async Task<AttendanceDto> Handle(
        CheckInQrCommand command,
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

        var verified = await registrationClient.VerifyTicketAsync(
            command.EventId,
            command.QrCode.Trim(),
            command.BearerToken,
            cancellationToken);

        if (verified is null || !verified.IsValid)
        {
            throw new ValidationException(verified?.Message ?? "Ticket verification failed.");
        }

        var existing = await db.AttendanceRecords.SingleOrDefaultAsync(
            x => x.EventId == command.EventId
                 && x.RegistrationId == verified.RegistrationId
                 && x.SessionId == command.SessionId,
            cancellationToken);

        if (existing is not null)
        {
            return ToDto(existing);
        }

        var entity = new AttendanceRecord
        {
            EventId = command.EventId,
            RegistrationId = verified.RegistrationId,
            ParticipantId = verified.ParticipantId,
            ParticipantUserId = verified.ParticipantUserId,
            SectionId = command.SectionId,
            SessionId = command.SessionId,
            StaffUserId = actorUserId,
            Method = command.Method,
            CheckedInAtUtc = DateTime.UtcNow
        };

        db.AttendanceRecords.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(entity);
    }

    private static AttendanceDto ToDto(AttendanceRecord record) => new(
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
