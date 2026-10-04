using EventFlow.Registration.Application.Features.Registrations.Commands.CancelRegistration;
using EventFlow.Registration.Application.Services;
using EventFlow.Registration.Domain.Enums;
using EventFlow.Registration.Infrastructure.Persistence;
using EventFlow.Registration.Infrastructure.Persistence.Repositories;
using EventFlow.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using Xunit;

using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Tests;

public sealed class CancelRegistrationCommandHandlerTests
{
    private readonly RegistrationDbContext _db;
    private readonly Guid _eventId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    public CancelRegistrationCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<RegistrationDbContext>()
            .UseInMemoryDatabase($"cancel-{Guid.NewGuid()}")
            .Options;

        _db = new RegistrationDbContext(options);
    }

    private CancelRegistrationCommandHandler CreateHandler() =>
        new(
            new RegistrationRepository(_db),
            new UnitOfWork(_db),
            new WaitlistPromotionService(
                new RegistrationRepository(_db),
                new RegistrationFormRepository(_db),
                new UnitOfWork(_db)),
            new FakeCurrentUserService(_userId));

    private void SetForm(CapacityMode mode, int? capacity)
    {
        _db.RegistrationForms.Add(new EventFlow.Registration.Domain.Entities.RegistrationForm
        {
            Id = Guid.NewGuid(),
            EventId = _eventId,
            CapacityMode = mode,
            Capacity = capacity
        });
    }

    private RegistrationEntity CreateRegistration(
        Guid userId,
        RegistrationStatus status,
        int? waitlistPosition)
    {
        var registration = new RegistrationEntity
        {
            Id = Guid.NewGuid(),
            EventId = _eventId,
            UserId = userId,
            RegistrationNumber = $"REG-{Guid.NewGuid():N}"[..16],
            Status = status,
            RegisteredAtUtc = DateTime.UtcNow.AddMinutes(-(waitlistPosition ?? 10)),
            WaitlistPosition = waitlistPosition,
            WaitlistedAtUtc = waitlistPosition is null
                ? null
                : DateTime.UtcNow.AddMinutes(-waitlistPosition.Value)
        };

        registration.Participant = new EventFlow.Registration.Domain.Entities.Participant
        {
            Id = Guid.NewGuid(),
            RegistrationId = registration.Id,
            EventId = _eventId,
            UserId = userId,
            FirstName = "Test",
            LastName = "Participant",
            Email = "test@example.com"
        };

        if (status == RegistrationStatus.Approved)
        {
            registration.Ticket = new EventFlow.Registration.Domain.Entities.Ticket
            {
                RegistrationId = registration.Id,
                ParticipantId = registration.Participant.Id,
                TicketNumber = $"TKT-{Guid.NewGuid():N}"[..16],
                QrCodeValue = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                IssuedAtUtc = DateTime.UtcNow
            };
        }

        _db.Registrations.Add(registration);
        return registration;
    }

    [Fact]
    public async Task Cancelling_approved_registration_promotes_next_waitlisted()
    {
        SetForm(CapacityMode.Limited, capacity: 1);
        var approved = CreateRegistration(_userId, RegistrationStatus.Approved, null);
        var waitlisted = CreateRegistration(Guid.NewGuid(), RegistrationStatus.Waitlisted, 1);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await CreateHandler().Handle(
            new CancelRegistrationCommand(
                _eventId,
                approved.Id,
                new EventFlow.Registration.Application.Contracts.Registrations.CancelRegistrationRequest
                {
                    Reason = "plans changed"
                }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var stored = await _db.Registrations
            .Include(x => x.Ticket)
            .ToListAsync(CancellationToken.None);

        var cancelled = stored.Single(x => x.Id == approved.Id);
        Assert.Equal(RegistrationStatus.Cancelled, cancelled.Status);
        Assert.NotNull(cancelled.Ticket);
        Assert.False(cancelled.Ticket.IsActive);

        var promoted = stored.Single(x => x.Id == waitlisted.Id);
        Assert.Equal(RegistrationStatus.Approved, promoted.Status);
        Assert.NotNull(promoted.Ticket);
        Assert.Null(promoted.WaitlistPosition);
    }

    [Fact]
    public async Task Cancelling_waitlisted_registration_promotes_nothing()
    {
        SetForm(CapacityMode.Limited, capacity: 1);
        CreateRegistration(Guid.NewGuid(), RegistrationStatus.Approved, null);
        var first = CreateRegistration(_userId, RegistrationStatus.Waitlisted, 1);
        var second = CreateRegistration(Guid.NewGuid(), RegistrationStatus.Waitlisted, 2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await CreateHandler().Handle(
            new CancelRegistrationCommand(
                _eventId,
                first.Id,
                new EventFlow.Registration.Application.Contracts.Registrations.CancelRegistrationRequest
                {
                    Reason = null
                }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var stored = await _db.Registrations.ToListAsync(CancellationToken.None);

        Assert.Equal(RegistrationStatus.Cancelled, stored.Single(x => x.Id == first.Id).Status);
        Assert.Equal(RegistrationStatus.Waitlisted, stored.Single(x => x.Id == second.Id).Status);
        Assert.Equal(1, stored.Count(x => x.Status == RegistrationStatus.Approved));
    }

    [Fact]
    public async Task Cancelling_someone_elses_registration_fails()
    {
        var other = CreateRegistration(Guid.NewGuid(), RegistrationStatus.Approved, null);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await CreateHandler().Handle(
            new CancelRegistrationCommand(
                _eventId,
                other.Id,
                new EventFlow.Registration.Application.Contracts.Registrations.CancelRegistrationRequest
                {
                    Reason = null
                }),
            CancellationToken.None);

        Assert.False(result.IsSuccess);

        var stored = await _db.Registrations.SingleAsync(
            x => x.Id == other.Id,
            CancellationToken.None);
        Assert.Equal(RegistrationStatus.Approved, stored.Status);
    }

    private sealed class FakeCurrentUserService(Guid userId) : ICurrentUserService
    {
        public bool IsAuthenticated => true;
        public Guid UserId => userId;
    }
}
