using EventFlow.Registration.Application.Features.Certificates.Queries.GetMyCertificatesAcrossEvents;
using EventFlow.Registration.Infrastructure.Persistence;
using EventFlow.Registration.Infrastructure.Persistence.Repositories;
using EventFlow.Registration.Domain.Enums;
using EventFlow.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using Xunit;

using CertificateEntity = EventFlow.Registration.Domain.Entities.Certificate;

namespace EventFlow.Registration.Tests;

public sealed class GetMyCertificatesAcrossEventsQueryHandlerTests
{
    private readonly RegistrationDbContext _db;
    private readonly Guid _userId = Guid.NewGuid();

    public GetMyCertificatesAcrossEventsQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<RegistrationDbContext>()
            .UseInMemoryDatabase($"my-certificates-{Guid.NewGuid()}")
            .Options;

        _db = new RegistrationDbContext(options);
    }

    private GetMyCertificatesAcrossEventsQueryHandler CreateHandler() =>
        new(new CertificateRepository(_db), new FakeCurrentUserService(_userId));

    private CertificateEntity AddCertificate(Guid userId, string eventName)
    {
        var certificate = new CertificateEntity
        {
            Id = Guid.NewGuid(),
            EventId = Guid.NewGuid(),
            RegistrationId = Guid.NewGuid(),
            ParticipantId = Guid.NewGuid(),
            UserId = userId,
            CertificateNumber = $"CERT-{Guid.NewGuid():N}"[..16],
            ParticipantName = "Test Participant",
            ParticipantEmail = "test@example.com",
            EventName = eventName,
            DocumentFileName = "certificate.pdf",
            Status = CertificateStatus.Generated,
            IssuedAtUtc = DateTime.UtcNow,
            GeneratedByUserId = Guid.NewGuid()
        };

        _db.Certificates.Add(certificate);
        _db.SaveChanges();

        return certificate;
    }

    [Fact]
    public async Task Returns_certificates_of_every_event_for_the_current_user()
    {
        AddCertificate(_userId, "First Event");
        AddCertificate(_userId, "Second Event");
        AddCertificate(Guid.NewGuid(), "Someone Else Event");

        var result = await CreateHandler().Handle(
            new GetMyCertificatesAcrossEventsQuery(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count);
        Assert.DoesNotContain(result.Data, x => x.EventName == "Someone Else Event");
        Assert.Contains(result.Data, x => x.EventName == "First Event");
        Assert.Contains(result.Data, x => x.EventName == "Second Event");
    }

    [Fact]
    public async Task Returns_empty_list_when_the_user_has_no_certificates()
    {
        AddCertificate(Guid.NewGuid(), "Someone Else Event");

        var result = await CreateHandler().Handle(
            new GetMyCertificatesAcrossEventsQuery(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data!);
    }

    private sealed class FakeCurrentUserService(Guid userId) : ICurrentUserService
    {
        public bool IsAuthenticated => true;
        public Guid UserId => userId;
    }
}
