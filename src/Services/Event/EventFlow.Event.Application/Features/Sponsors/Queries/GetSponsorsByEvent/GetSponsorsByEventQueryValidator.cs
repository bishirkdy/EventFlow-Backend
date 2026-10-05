using FluentValidation;

namespace EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorsByEvent;

public sealed class GetSponsorsByEventQueryValidator
    : AbstractValidator<GetSponsorsByEventQuery>
{
    public GetSponsorsByEventQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
