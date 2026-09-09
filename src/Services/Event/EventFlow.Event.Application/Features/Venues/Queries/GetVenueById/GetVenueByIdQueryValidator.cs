
using FluentValidation;

namespace EventFlow.Event.Application.Features.Venues.Queries.GetVenueById
{
    public sealed class GetVenueByIdQueryValidator: AbstractValidator<GetVenueByIdQuery>
    {
        public GetVenueByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Venue ID is required.");

            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
