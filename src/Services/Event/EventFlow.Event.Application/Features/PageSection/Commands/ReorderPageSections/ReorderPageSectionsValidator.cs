

using FluentValidation;

namespace EventFlow.Event.Application.Features.PageSection.Commands.ReorderPageSections
{
    public sealed class ReorderPageSectionsValidator: AbstractValidator<ReorderPageSectionsCommand>
    {
        public ReorderPageSectionsValidator()
        {
            RuleFor(x => x.PageId)
                .NotEmpty()
                .WithMessage("Page ID is required.");

            RuleFor(x => x.SectionIds)
                .NotEmpty()
                .WithMessage("Section IDs are required.");

            RuleFor(x => x.SectionIds)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("Duplicate section IDs are not allowed.");
        }
    }
}
