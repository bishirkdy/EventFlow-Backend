using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Features.Feedback.Commands.SubmitFeedback;
using EventFlow.Event.Domain.Entities;
using EventFlow.Event.Domain.Enums;
using EventFlow.Event.Infrastructure.Persistence;
using EventFlow.Event.Infrastructure.Persistence.Repositories;
using EventFlow.SharedKernel.Exceptions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EventFlow.Event.Tests;

public sealed class SubmitFeedbackCommandHandlerTests
{
    private readonly EventCoreDbContext _db;
    private readonly SubmitFeedbackCommandHandler _handler;
    private readonly Guid _eventId = Guid.NewGuid();

    public SubmitFeedbackCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<EventCoreDbContext>()
            .UseInMemoryDatabase($"feedback-{Guid.NewGuid()}")
            .Options;

        _db = new EventCoreDbContext(options);

        _db.EventFeatures.Add(new EventFeature(_eventId, FeatureIds.Feedback, isEnabled: true));
        _db.SaveChanges();

        _handler = new SubmitFeedbackCommandHandler(
            new FeedbackRepository(_db),
            new EventFeatureRepository(_db),
            new SessionRepository(_db),
            new SpeakerRepository(_db),
            new VenueRepository(_db),
            new UnitOfWork(_db));
    }

    private SubmitFeedbackCommand EventCommand(int rating = 5, string? comment = null) =>
        new(_eventId, Guid.NewGuid(), FeedbackTargetType.Event, null, rating, comment);

    [Fact]
    public async Task Event_level_feedback_is_stored()
    {
        var command = EventCommand(rating: 4, comment: "Nice");

        var response = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(4, response.Rating);
        Assert.Equal(1, await _db.Feedbacks.CountAsync());
        var stored = await _db.Feedbacks.SingleAsync();
        Assert.Equal(_eventId, stored.TargetId);
        Assert.Equal("Nice", stored.Comment);
    }

    [Fact]
    public async Task Duplicate_feedback_for_same_target_conflicts()
    {
        var participant = Guid.NewGuid();
        var first = new SubmitFeedbackCommand(
            _eventId, participant, FeedbackTargetType.Event, null, 5, null);
        var second = first with { Rating = 1 };

        await _handler.Handle(first, CancellationToken.None);

        await Assert.ThrowsAsync<ConflictException>(
            () => _handler.Handle(second, CancellationToken.None));
    }

    [Fact]
    public async Task Disabled_feature_rejects_submission()
    {
        var disabledEvent = Guid.NewGuid();
        _db.EventFeatures.Add(new EventFeature(disabledEvent, FeatureIds.Feedback, isEnabled: false));
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new SubmitFeedbackCommand(
            disabledEvent, Guid.NewGuid(), FeedbackTargetType.Event, null, 5, null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Unknown_session_target_rejects_submission()
    {
        var command = new SubmitFeedbackCommand(
            _eventId,
            Guid.NewGuid(),
            FeedbackTargetType.Session,
            Guid.NewGuid(),
            5,
            null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Session_target_feedback_is_stored_with_session_id()
    {
        var session = new Session(
            _eventId,
            Guid.NewGuid(),
            "Keynote",
            null,
            "talk",
            null,
            null,
            null,
            null);
        _db.Sessions.Add(session);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new SubmitFeedbackCommand(
            _eventId,
            Guid.NewGuid(),
            FeedbackTargetType.Session,
            session.Id,
            3,
            "Solid talk");

        await _handler.Handle(command, CancellationToken.None);

        var stored = await _db.Feedbacks.SingleAsync();
        Assert.Equal(session.Id, stored.TargetId);
        Assert.Equal(FeedbackTargetType.Session, stored.TargetType);
    }
}
