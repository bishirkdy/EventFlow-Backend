using EventFlow.Event.Application.Abstractions.Storage;
using FluentValidation;

namespace EventFlow.Event.Application.Features.Speakers.Commands.UpdateSpeaker;

public sealed class UpdateSpeakerCommandValidator : AbstractValidator<UpdateSpeakerCommand>
{
    public UpdateSpeakerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Speaker ID is required.");

        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Speaker name is required.")
            .MaximumLength(200)
            .WithMessage("Speaker name cannot exceed 200 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(2000)
            .WithMessage("Speaker bio cannot exceed 2000 characters.");

        RuleFor(x => x.Designation)
            .MaximumLength(200)
            .WithMessage("Speaker designation cannot exceed 200 characters.");

        RuleFor(x => x.Organization)
            .MaximumLength(200)
            .WithMessage("Speaker organization cannot exceed 200 characters.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format.")
            .MaximumLength(320)
            .WithMessage("Speaker email cannot exceed 320 characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order cannot be negative.");

        RuleFor(x => x.Image)
            .Must(image => image is null || image.Length > 0)
            .WithMessage("Speaker image cannot be empty.");
    }
}
