

using FluentValidation;

namespace EventFlow.Event.Application.Features.PageSection.Commands.DeletePageSection
{
    public sealed class DeletePageSectionValidator: AbstractValidator<DeletePageSectionCommand>
    {
        public DeletePageSectionValidator()
        {
            RuleFor(x => x.PageId)
                .NotEmpty()
                .WithMessage("Page ID is required.");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Section ID is required.");
        }
    }
}
