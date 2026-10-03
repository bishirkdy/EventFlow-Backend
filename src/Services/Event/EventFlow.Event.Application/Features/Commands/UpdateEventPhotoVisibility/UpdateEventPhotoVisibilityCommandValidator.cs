using FluentValidation;

namespace EventFlow.Event.Application.Features.Commands.UpdateEventPhotoVisibility
{
    public sealed class UpdateEventPhotoVisibilityCommandValidator : AbstractValidator<UpdateEventPhotoVisibilityCommand>
    {
        public UpdateEventPhotoVisibilityCommandValidator()
        {
            RuleFor(x => x.PhotoId)
                .NotEmpty()
                .WithMessage("Photo ID is required.");

            RuleFor(x => x.ApprovedBy)
                .NotEmpty()
                .WithMessage("ApprovedBy user ID is required.");
        }
    }
}