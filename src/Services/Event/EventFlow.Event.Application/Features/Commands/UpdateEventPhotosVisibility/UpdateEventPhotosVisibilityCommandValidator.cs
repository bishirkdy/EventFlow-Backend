using FluentValidation;

namespace EventFlow.Event.Application.Features.Commands.UpdateEventPhotosVisibility;

public sealed class UpdateEventPhotosVisibilityCommandValidator : AbstractValidator<UpdateEventPhotosVisibilityCommand>
{
    private const int MaxBatchSize = 200;

    public UpdateEventPhotosVisibilityCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.PhotoIds)
            .NotNull()
            .WithMessage("At least one photo ID is required.")
            .NotEmpty()
            .WithMessage("At least one photo ID is required.")
            .Must(ids => ids.Count <= MaxBatchSize)
            .WithMessage($"You can update at most {MaxBatchSize} photos at once.");

        RuleFor(x => x.ApprovedBy)
            .NotEmpty()
            .WithMessage("ApprovedBy user ID is required.");
    }
}
