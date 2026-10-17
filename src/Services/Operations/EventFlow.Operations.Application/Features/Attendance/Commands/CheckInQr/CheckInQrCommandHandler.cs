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

        // Session tickets are only valid at the session they were registered for.
        if (verified.ScopeType == RegistrationScope.Session &&
            command.SessionId != verified.ScopeId)
        {
            throw new ValidationException(
                "This ticket is only valid for the session it was registered for.");
        }

        var existing = await db.AttendanceRecords.SingleOrDefaultAsync(
            x => x.EventId == command.EventId
                 && x.RegistrationId == verified.RegistrationId
                 && x.SessionId == command.SessionId,
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
            command.SectionId,
            command.SessionId,
            actorUserId,
            command.Method);

        db.AttendanceRecords.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<AttendanceDto>(entity);
    }
}
