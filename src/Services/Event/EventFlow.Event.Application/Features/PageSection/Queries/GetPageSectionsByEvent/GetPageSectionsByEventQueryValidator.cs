using FluentValidation;

namespace EventFlow.Event.Application.Features.PageSection.Queries.GetPageSectionsByEvent;

public sealed class GetPageSectionsByEventQueryValidator
    : AbstractValidator<GetPageSectionsByEventQuery>
{
    public GetPageSectionsByEventQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
