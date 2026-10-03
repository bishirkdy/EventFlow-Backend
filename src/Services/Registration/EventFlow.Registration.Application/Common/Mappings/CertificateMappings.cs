using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Domain.Entities;

namespace EventFlow.Registration.Application.Common.Mappings;

public static class CertificateMappings
{
    public static CertificateDto ToDto(this Certificate certificate) => new()
    {
        Id = certificate.Id,
        EventId = certificate.EventId,
        RegistrationId = certificate.RegistrationId,
        CertificateNumber = certificate.CertificateNumber,
        ParticipantName = certificate.ParticipantName,
        ParticipantEmail = certificate.ParticipantEmail,
        EventName = certificate.EventName,
        Status = certificate.Status.ToString(),
        IssuedAtUtc = certificate.IssuedAtUtc,
        RevokedAtUtc = certificate.RevokedAtUtc
    };
}
