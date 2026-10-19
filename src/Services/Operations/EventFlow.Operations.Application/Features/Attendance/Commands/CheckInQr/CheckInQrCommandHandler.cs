using AutoMapper;
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
    IEventScheduleClient scheduleClient,
    IRegistrationClient registrationClient,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<CheckInQrCommand, AttendanceDto>
{
    public async Task<AttendanceDto> Handle(
        CheckInQrCommand command,
        CancellationToken cancellationToken)
    {
        var actorUserId = currentUser.UserId;

        var verified = await registrationClient.VerifyTicketAsync(
            command.EventId,
            command.QrCode.Trim(),
            command.BearerToken,
            cancellationToken);

        if (verified is null || !verified.IsValid)
        {
            throw new ValidationException(verified?.Message ?? "Ticket verification failed.");
        }

        // The scanned ticket decides the scope: a session ticket is always
        // recorded against its own session, an event ticket against the event.
        var sessionId = verified.ScopeType == RegistrationScope.Session
            ? verified.ScopeId
            : null;

        var access = await AttendanceAuthorization.ResolveAsync(
            db,
            authorization,
            command.EventId,
            actorUserId,
            sessionId,
            cancellationToken);

        if (access == AttendanceAccess.None)
        {
            throw new ForbiddenException("You are not assigned to this attendance scope.");
        }

        var sectionId = sessionId is { } scannedSessionId
            ? await scheduleClient.GetSessionSectionIdAsync(
                command.EventId, scannedSessionId, cancellationToken)
            : null;

        var existing = await db.AttendanceRecords.SingleOrDefaultAsync(
            x => x.EventId == command.EventId
                 && x.RegistrationId == verified.RegistrationId
                 && x.SessionId == sessionId,
            cancellationToken);

        // Idempotent: return the record if the ticket was already checked in.
        if (existing is not null)
        {
            return mapper.Map<AttendanceDto>(existing);
        }

        var entity = new AttendanceRecord(
            command.EventId,
            verified.RegistrationId,
            verified.ParticipantId,
            verified.ParticipantUserId,
            sectionId,
            sessionId,
            actorUserId,
            command.Method);

        db.AttendanceRecords.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<AttendanceDto>(entity);
    }
}
