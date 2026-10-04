using EventFlow.Event.Application.Features.Feedback.Commands.SubmitFeedback;
using EventFlow.Event.Domain.Enums;
using FluentValidation.Results;
using Xunit;

namespace EventFlow.Event.Tests;

public sealed class SubmitFeedbackCommandValidatorTests
{
    private readonly SubmitFeedbackCommandValidator _validator = new();

    private static SubmitFeedbackCommand ValidCommand() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            FeedbackTargetType.Event,
            null,
            5,
            "Great event!");

    [Fact]
    public void Valid_command_passes()
    {
        var result = _validator.Validate(ValidCommand());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Rating_out_of_range_fails()
    {
        var command = ValidCommand() with { Rating = 7 };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Rating must be between 1 and 5.");
    }

    [Fact]
    public void Missing_target_for_session_feedback_fails()
    {
        var command = ValidCommand() with
        {
            TargetType = FeedbackTargetType.Session,
            TargetId = null
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => e.ErrorMessage == "A target is required for session, speaker and venue feedback.");
    }

    [Fact]
    public void Comment_too_long_fails()
    {
        var command = ValidCommand() with { Comment = new string('x', 2001) };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Comment cannot exceed 2000 characters.");
    }

    [Fact]
    public void Empty_event_id_fails()
    {
        var command = ValidCommand() with { EventId = Guid.Empty };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Event ID is required.");
    }
}
