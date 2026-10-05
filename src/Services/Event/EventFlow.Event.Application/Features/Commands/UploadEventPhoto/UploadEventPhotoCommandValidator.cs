using FluentValidation;

namespace EventFlow.Event.Application.Features.Commands.UploadEventPhoto;

public sealed class UploadEventPhotoCommandValidator
    : AbstractValidator<UploadEventPhotoCommand>
{
    public UploadEventPhotoCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.File)
            .Must(file => file is not null && file.Length > 0)
            .WithMessage("A photo file is required.");
    }
}
