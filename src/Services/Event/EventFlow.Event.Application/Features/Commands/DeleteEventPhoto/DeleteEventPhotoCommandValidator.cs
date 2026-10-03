using FluentValidation;

namespace EventFlow.Event.Application.Features.Commands.DeleteEventPhoto
{
    public sealed class DeleteEventPhotoCommandValidator : AbstractValidator<DeleteEventPhotoCommand>
    {
        public DeleteEventPhotoCommandValidator()
        {
            RuleFor(x => x.PhotoId)
                .NotEmpty()
                .WithMessage("Photo ID is required.");

            RuleFor(x => x.RequestedBy)
                .NotEmpty()
                .WithMessage("RequestedBy user ID is required.");
        }
    }
}