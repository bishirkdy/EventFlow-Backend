using FluentValidation;

namespace EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorById;

public sealed class GetSponsorByIdQueryValidator : AbstractValidator<GetSponsorByIdQuery>
{
    public GetSponsorByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sponsor ID is required.");

        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
