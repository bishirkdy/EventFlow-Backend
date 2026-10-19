using EventFlow.Contracts.Common;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Features.Queries.GetEventPhotos;
using EventFlow.Event.Domain.Entities;
using EventFlow.Security.Authentication;

namespace EventFlow.Event.Application.Tests.Features.Queries;

public sealed class GetEventPhotosQueryHandlerTests
{
    private static readonly Guid EventId = Guid.NewGuid();
    private static readonly CancellationToken Token = CancellationToken.None;

    [Fact]
    public async Task Anonymous_caller_is_forced_to_visible_photos_even_when_asking_for_hidden()
    {
        var repository = new FakeEventPhotoRepository();
        var handler = new GetEventPhotosQueryHandler(
            repository,
            new FakeEventFeatureRepository(),
            new FakeCurrentUserService { IsAuthenticated = false });

        await handler.Handle(
            new GetEventPhotosQuery(EventId, 1, 20, VisibleOnly: false),
            Token);

        Assert.True(repository.LastVisibleOnly);
    }

    [Fact]
    public async Task Anonymous_caller_defaults_to_visible_photos()
    {
        var repository = new FakeEventPhotoRepository();
        var handler = new GetEventPhotosQueryHandler(
            repository,
            new FakeEventFeatureRepository(),
            new FakeCurrentUserService { IsAuthenticated = false });

        await handler.Handle(new GetEventPhotosQuery(EventId), Token);

        Assert.True(repository.LastVisibleOnly);
    }

    [Fact]
    public async Task Authenticated_caller_can_request_hidden_photos()
    {
        var repository = new FakeEventPhotoRepository();
        var handler = new GetEventPhotosQueryHandler(
            repository,
            new FakeEventFeatureRepository(),
            new FakeCurrentUserService { IsAuthenticated = true });

        await handler.Handle(
            new GetEventPhotosQuery(EventId, 1, 20, VisibleOnly: false),
            Token);

        Assert.False(repository.LastVisibleOnly);
    }

    [Fact]
    public async Task Authenticated_caller_can_request_every_photo()
    {
        var repository = new FakeEventPhotoRepository();
        var handler = new GetEventPhotosQueryHandler(
            repository,
            new FakeEventFeatureRepository(),
            new FakeCurrentUserService { IsAuthenticated = true });

        await handler.Handle(
            new GetEventPhotosQuery(EventId, 1, 20, VisibleOnly: null),
            Token);

        Assert.Null(repository.LastVisibleOnly);
    }

    private sealed class FakeCurrentUserService : ICurrentUserService
    {
        public bool IsAuthenticated { get; init; }
        public Guid UserId => Guid.NewGuid();
        public bool IsPlatformAdmin => false;
    }

    private sealed class FakeEventPhotoRepository : IEventPhotoRepository
    {
        public bool? LastVisibleOnly { get; private set; }

        public Task<PaginatedResponse<EventPhoto>> GetPagedByEventIdAsync(
            Guid eventId,
            int page,
            int pageSize,
            bool? visibleOnly,
            CancellationToken cancellationToken = default)
        {
            LastVisibleOnly = visibleOnly;
            return Task.FromResult(new PaginatedResponse<EventPhoto>
            {
                Items = [],
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = 0
            });
        }

        public Task<IReadOnlyList<EventPhoto>> GetByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EventPhoto>>([]);

        public Task<IReadOnlyList<EventPhoto>> GetVisibleByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EventPhoto>>([]);

        public Task<IReadOnlyList<EventPhoto>> GetByPhotographerIdAsync(
            Guid photographerId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EventPhoto>>([]);

        public Task<EventPhoto?> GetByIdWithEventAsync(
            Guid photoId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<EventPhoto?>(null);

        public Task<IReadOnlyList<EventPhoto>> GetByIdsAsync(
            IReadOnlyList<Guid> photoIds,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EventPhoto>>([]);

        public Task<EventPhoto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<EventPhoto?>(null);

        public Task AddAsync(EventPhoto entity, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public void Update(EventPhoto entity)
        {
        }

        public void Remove(EventPhoto entity)
        {
        }
    }

    private sealed class FakeEventFeatureRepository : IEventFeatureRepository
    {
        public Task<EventFeature?> GetByEventAndFeatureAsync(
            Guid eventId,
            Guid featureId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<EventFeature?>(new EventFeature(eventId, featureId));

        public Task<IReadOnlyList<EventFeature>> GetByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EventFeature>>([]);

        public Task<EventFeature?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<EventFeature?>(null);

        public Task AddAsync(EventFeature entity, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public void Update(EventFeature entity)
        {
        }

        public void Remove(EventFeature entity)
        {
        }
    }
}
