

using FluentValidation;

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetCompleteEventPageBySlug
{
    public sealed class GetCompleteEventPageBySlugValidator: AbstractValidator<GetCompleteEventPageBySlugQuery>
    {
        public GetCompleteEventPageBySlugValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Slug)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Slug is required.");
        }
    }
}
