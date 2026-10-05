using FluentValidation;

namespace EventFlow.Event.Application.Features.PageSection.Commands.CreatePageSection
{
    public sealed class CreatePageSectionCommandValidator : AbstractValidator<CreatePageSectionCommand>
    {
        public CreatePageSectionCommandValidator()
        {
            RuleFor(x => x.PageId)
                .NotEmpty()
                .WithMessage("Page ID is required.");

            RuleFor(x => x.SectionType)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Section type is required.");

            RuleFor(x => x.Title)
                .MaximumLength(200);

            RuleFor(x => x.Content)
                .MaximumLength(10000);

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1000);
        }
    }
}
