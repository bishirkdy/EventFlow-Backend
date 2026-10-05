

using FluentValidation;

namespace EventFlow.Event.Application.Features.PageSection.Queries.GetPageSections
{
    public sealed class GetPageSectionsQueryValidator: AbstractValidator<GetPageSectionsQuery>
    {
        public GetPageSectionsQueryValidator()
        {
            RuleFor(x => x.PageId)
                .NotEmpty()
                .WithMessage("Page ID is required.");
        }
    }
}
