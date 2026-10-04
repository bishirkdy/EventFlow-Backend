using AutoMapper;
using EventFlow.Event.Application.Abstractions.Services;
using EventFlow.Event.Application.Common.Mappings;
using EventFlow.Event.Application.Features.Events.Queries.GetMyEvents;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Domain.Enums;
using EventFlow.Event.Infrastructure.Persistence;
using EventFlow.Event.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EventFlow.Event.Tests;

public sealed class GetMyEventsQueryHandlerTests
{
    private readonly EventCoreDbContext _db;
    private readonly GetMyEventsQueryHandler _handler;
    private readonly FakeUserDirectory _userDirectory = new();
    private readonly FakeRegistrationClient _registrationClient = new();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _eventTypeId = Guid.NewGuid();

    public GetMyEventsQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<EventCoreDbContext>()
            .UseInMemoryDatabase($"my-events-{Guid.NewGuid()}")
            .Options;

        _db = new EventCoreDbContext(options);
        _db.EventTypes.Add(new EventType(_eventTypeId, EventTypeCode.Conference, "Conference", null));
        _db.SaveChanges();

        var mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<EventMappingProfile>(),
            Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance)
            .CreateMapper();

        _handler = new GetMyEventsQueryHandler(
            new EventRepository(_db),
            mapper,
            _userDirectory,
            _registrationClient);
    }

    private EventEntity AddEvent(string name, Guid createdBy)
    {
        var entity = new EventEntity(
            name,
            null,
            _eventTypeId,
            null,
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(8),
            "UTC",
            createdBy);

        _db.EventEntities.Add(entity);
        _db.SaveChanges();

        return entity;
    }

    [Fact]
    public async Task Returns_created_role_and_registered_events()
    {
        var created = AddEvent("Created by me", _userId);
        var role = AddEvent("Staff on this", Guid.NewGuid());
        var registered = AddEvent("Registered for this", Guid.NewGuid());

        _userDirectory.EventIds.Add(role.Id);
        _registrationClient.EventIds.Add(registered.Id);

        var result = await _handler.Handle(
            new GetMyEventsQuery(_userId),
            CancellationToken.None);

        Assert.Equal(3, result.Count);
        Assert.Contains(result, x => x.Id == created.Id);
        Assert.Contains(result, x => x.Id == role.Id);
        Assert.Contains(result, x => x.Id == registered.Id);
    }

    [Fact]
    public async Task Same_event_from_several_sources_is_returned_once()
    {
        var shared = AddEvent("Shared event", _userId);

        _userDirectory.EventIds.Add(shared.Id);
        _registrationClient.EventIds.Add(shared.Id);

        var result = await _handler.Handle(
            new GetMyEventsQuery(_userId),
            CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(shared.Id, result[0].Id);
    }

    [Fact]
    public async Task Created_and_role_events_survive_registration_service_failure()
    {
        var created = AddEvent("Created by me", _userId);
        var role = AddEvent("Staff on this", Guid.NewGuid());
        AddEvent("Registered for this", Guid.NewGuid());

        _userDirectory.EventIds.Add(role.Id);
        _registrationClient.Fail = true;

        var result = await _handler.Handle(
            new GetMyEventsQuery(_userId),
            CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Id == created.Id);
        Assert.Contains(result, x => x.Id == role.Id);
    }

    private sealed class FakeUserDirectory : IUserDirectoryClient
    {
        public List<Guid> EventIds { get; } = [];

        public Task<string?> GetDisplayNameAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>("Test User");

        public Task AssignOwnerAsync(
            Guid userId,
            Guid eventId,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<IReadOnlyList<Guid>> GetEventIdsForUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Guid>>(EventIds);
    }

    private sealed class FakeRegistrationClient : IRegistrationClient
    {
        public List<Guid> EventIds { get; } = [];
        public bool Fail { get; set; }

        public Task<IReadOnlyList<Guid>> GetEventIdsForUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            Fail
                ? throw new InvalidOperationException("Registration service is down.")
                : Task.FromResult<IReadOnlyList<Guid>>(EventIds);
    }
}
