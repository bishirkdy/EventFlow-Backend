using FluentValidation;

namespace EventFlow.Event.Application.Features.PageSection.Commands.CreatePageSection
{
    public sealed class CreatePageSectionValidator: AbstractValidator<CreatePageSectionCommand>
    {
        public CreatePageSectionValidator()
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

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Display order must be zero or greater.");
        }
    }
}
