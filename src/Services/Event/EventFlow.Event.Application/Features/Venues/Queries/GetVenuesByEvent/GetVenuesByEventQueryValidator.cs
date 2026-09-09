using FluentValidation;


namespace EventFlow.Event.Application.Features.Venues.Queries.GetVenuesByEvent
{
    public sealed class GetVenuesByEventQueryValidator: AbstractValidator<GetVenuesByEventQuery>
    {
        public GetVenuesByEventQueryValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
