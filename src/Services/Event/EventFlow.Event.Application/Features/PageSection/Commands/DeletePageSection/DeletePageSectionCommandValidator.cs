

using FluentValidation;

namespace EventFlow.Event.Application.Features.PageSection.Commands.DeletePageSection
{
    public sealed class DeletePageSectionCommandValidator: AbstractValidator<DeletePageSectionCommand>
    {
        public DeletePageSectionCommandValidator()
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
