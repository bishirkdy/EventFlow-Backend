using FluentValidation;

namespace EventFlow.Event.Application.Features.PageSection.Commands.UpdatePageSection
{
    public sealed class UpdatePageSectionCommandValidator : AbstractValidator<UpdatePageSectionCommand>
    {
        public UpdatePageSectionCommandValidator()
        {
            RuleFor(x => x.PageId)
                .NotEmpty()
                .WithMessage("Page ID is required.");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Section ID is required.");

            RuleFor(x => x.SectionType)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Title)
                .MaximumLength(200);

            RuleFor(x => x.Content)
                .MaximumLength(10000);

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1000);
        }
    }
}
