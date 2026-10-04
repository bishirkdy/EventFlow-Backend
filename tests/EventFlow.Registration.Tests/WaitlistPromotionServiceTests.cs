using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Services;
using EventFlow.Registration.Domain.Enums;
using EventFlow.Registration.Infrastructure.Persistence;
using EventFlow.Registration.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Tests;

public sealed class WaitlistPromotionServiceTests
{
    private readonly RegistrationDbContext _db;
    private readonly WaitlistPromotionService _service;
    private readonly Guid _eventId = Guid.NewGuid();

    public WaitlistPromotionServiceTests()
    {
        var options = new DbContextOptionsBuilder<RegistrationDbContext>()
            .UseInMemoryDatabase($"waitlist-{Guid.NewGuid()}")
            .Options;

        _db = new RegistrationDbContext(options);
        _service = new WaitlistPromotionService(
            new RegistrationRepository(_db),
            new RegistrationFormRepository(_db),
            new UnitOfWork(_db));
    }

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

    private RegistrationEntity CreateRegistration(RegistrationStatus status, int? waitlistPosition)
    {
        var registration = new RegistrationEntity
        {
            Id = Guid.NewGuid(),
            EventId = _eventId,
            UserId = Guid.NewGuid(),
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
            UserId = registration.UserId,
            FirstName = "Test",
            LastName = "Participant",
            Email = "test@example.com"
        };

        _db.Registrations.Add(registration);
        return registration;
    }

    [Fact]
    public async Task Promotes_next_waitlisted_until_capacity_is_reached()
    {
        SetForm(CapacityMode.Limited, capacity: 2);
        var approved = CreateRegistration(RegistrationStatus.Approved, null);
        var first = CreateRegistration(RegistrationStatus.Waitlisted, 1);
        var second = CreateRegistration(RegistrationStatus.Waitlisted, 2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var promoted = await _service.PromoteWaitlistedUntilCapacityAsync(
            _eventId,
            CancellationToken.None);

        Assert.Equal(1, promoted);

        var stored = await _db.Registrations
            .Include(x => x.Ticket)
            .ToListAsync(CancellationToken.None);

        Assert.Equal(RegistrationStatus.Approved, stored.Single(x => x.Id == approved.Id).Status);

        var promotedFirst = stored.Single(x => x.Id == first.Id);
        Assert.Equal(RegistrationStatus.Approved, promotedFirst.Status);
        Assert.Null(promotedFirst.WaitlistPosition);
        Assert.Null(promotedFirst.WaitlistedAtUtc);
        Assert.NotNull(promotedFirst.Ticket);

        Assert.Equal(RegistrationStatus.Waitlisted, stored.Single(x => x.Id == second.Id).Status);
        Assert.Equal(2, await _db.Registrations.CountAsync(
            x => x.Status == RegistrationStatus.Approved,
            CancellationToken.None));
    }

    [Fact]
    public async Task Unlimited_capacity_promotes_nothing()
    {
        SetForm(CapacityMode.Unlimited, capacity: null);
        CreateRegistration(RegistrationStatus.Approved, null);
        CreateRegistration(RegistrationStatus.Waitlisted, 1);
        await _db.SaveChangesAsync(CancellationToken.None);

        var promoted = await _service.PromoteWaitlistedUntilCapacityAsync(
            _eventId,
            CancellationToken.None);

        Assert.Equal(0, promoted);
        Assert.Equal(1, await _db.Registrations.CountAsync(
            x => x.Status == RegistrationStatus.Waitlisted,
            CancellationToken.None));
    }

    [Fact]
    public async Task Full_capacity_promotes_nothing()
    {
        SetForm(CapacityMode.Limited, capacity: 1);
        CreateRegistration(RegistrationStatus.Approved, null);
        CreateRegistration(RegistrationStatus.Waitlisted, 1);
        await _db.SaveChangesAsync(CancellationToken.None);

        var promoted = await _service.PromoteWaitlistedUntilCapacityAsync(
            _eventId,
            CancellationToken.None);

        Assert.Equal(0, promoted);
    }

    [Fact]
    public async Task No_waitlisted_registrations_promotes_nothing()
    {
        SetForm(CapacityMode.Limited, capacity: 3);
        CreateRegistration(RegistrationStatus.Approved, null);
        await _db.SaveChangesAsync(CancellationToken.None);

        var promoted = await _service.PromoteWaitlistedUntilCapacityAsync(
            _eventId,
            CancellationToken.None);

        Assert.Equal(0, promoted);
    }
}
