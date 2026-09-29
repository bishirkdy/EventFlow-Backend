using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Application.Common.Mappings;

public static class RegistrationMapping
{
    public static RegistrationDto ToDto(this RegistrationEntity x) => new()
    {
        Id = x.Id,
        EventId = x.EventId,
        UserId = x.UserId,
        RegistrationNumber = x.RegistrationNumber,
        Status = x.Status,
        RegisteredAtUtc = x.RegisteredAtUtc,
        ApprovedAtUtc = x.ApprovedAtUtc,
        RejectedAtUtc = x.RejectedAtUtc,
        CancelledAtUtc = x.CancelledAtUtc,
        WaitlistedAtUtc = x.WaitlistedAtUtc,
        WaitlistPosition = x.WaitlistPosition,
        RejectionReason = x.RejectionReason,
        CancellationReason = x.CancellationReason,
        Participant = x.Participant?.ToDto(),
        Ticket = x.Ticket?.ToDto()
    }
    ;
    public static ParticipantDto ToDto(this Participant x) => new()
    {
        Id = x.Id,
        EventId = x.EventId,
        RegistrationId = x.RegistrationId,
        UserId = x.UserId,
        ParticipantNumber = x.ParticipantNumber,
        FirstName = x.FirstName,
        LastName = x.LastName,
        Email = x.Email,
        Phone = x.Phone,
        Organization = x.Organization,
        Designation = x.Designation,
        Status = x.Status
    }
    ;
    public static TicketDto ToDto(this Ticket x) => new()
    {
        Id = x.Id,
        RegistrationId = x.RegistrationId,
        ParticipantId = x.ParticipantId,
        TicketNumber = x.TicketNumber,
        QrCodeValue = x.QrCodeValue,
        IssuedAtUtc = x.IssuedAtUtc,
        RevokedAtUtc = x.RevokedAtUtc,
        IsActive = x.IsActive
    }
    ;
}
