using FluentValidation;

namespace EventFlow.Event.Application.Features.Commands.UploadEventPhoto
{
    public sealed class UploadEventPhotoCommandValidator : AbstractValidator<UploadEventPhotoCommand>
    {
        public UploadEventPhotoCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.PhotographerId)
                .NotEmpty()
                .WithMessage("Photographer ID is required.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty()
                .WithMessage("Image URL is required.")
                .MaximumLength(2000)
                .WithMessage("Image URL cannot exceed 2000 characters.");

            RuleFor(x => x.PublicId)
                .NotEmpty()
                .WithMessage("Public ID is required.")
                .MaximumLength(500)
                .WithMessage("Public ID cannot exceed 500 characters.");

            RuleFor(x => x.ThumbnailUrl)
                .MaximumLength(2000)
                .WithMessage("Thumbnail URL cannot exceed 2000 characters.");
        }
    }
}