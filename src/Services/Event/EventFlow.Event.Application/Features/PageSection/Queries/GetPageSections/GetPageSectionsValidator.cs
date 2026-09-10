

using FluentValidation;

namespace EventFlow.Event.Application.Features.PageSection.Queries.GetPageSections
{
    public sealed class GetPageSectionsValidator: AbstractValidator<GetPageSectionsQuery>
    {
        public GetPageSectionsValidator()
        {
            RuleFor(x => x.PageId)
                .NotEmpty()
                .WithMessage("Page ID is required.");
        }
    }
}
