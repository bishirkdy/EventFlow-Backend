using System.Security.Cryptography;
using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Domain;

namespace EventFlow.Registration.Domain.Entities;

public sealed class Ticket : Entity
{
    // EF Core materialization constructor.
    private Ticket()
    {
    }

    public Ticket(
        Guid registrationId,
        Guid participantId,
        string ticketNumber,
        string qrCodeValue,
        RegistrationScope scopeType = RegistrationScope.Event,
        Guid? scopeId = null)
    {
        RegistrationId = registrationId;
        ParticipantId = participantId;
        TicketNumber = ticketNumber;
        QrCodeValue = qrCodeValue;
        ScopeType = scopeType;
        ScopeId = scopeId;
        IssuedAtUtc = DateTime.UtcNow;
        IsActive = true;
    }

    // Issues a new active ticket with a generated number and QR value.
    public static Ticket Issue(
        Guid registrationId,
        Guid participantId,
        RegistrationScope scopeType = RegistrationScope.Event,
        Guid? scopeId = null)
    {
        var now = DateTime.UtcNow;

        return new Ticket(
            registrationId,
            participantId,
            $"TKT-{now:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}",
            GenerateQrCode(),
            scopeType,
            scopeId);
    }

    public Guid RegistrationId { get; private set; }

    public Guid ParticipantId { get; private set; }

    public RegistrationScope ScopeType { get; private set; } = RegistrationScope.Event;

    public Guid? ScopeId { get; private set; }

    public string TicketNumber { get; private set; } = "";

    public string QrCodeValue { get; private set; } = "";

    public DateTime IssuedAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public bool IsActive { get; private set; } = true;

    public Registration Registration { get; private set; } = null!;

    public Participant Participant { get; private set; } = null!;

    // Revokes the ticket so it can no longer be scanned.
    public void Revoke()
    {
        IsActive = false;
        RevokedAtUtc = DateTime.UtcNow;
        SetUpdatedAt();
    }

    // Produces a URL safe random token used as the QR payload.
    private static string GenerateQrCode() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
}
