
using FluentValidation;

namespace EventFlow.Event.Application.Features.Venues.Commands.UpdateVenue
{
    public sealed class UpdateVenueCommandValidator: AbstractValidator<UpdateVenueCommand>
    {
        public UpdateVenueCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Venue ID is required.");

            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Venue name is required.")
                .MaximumLength(200)
                .WithMessage("Venue name cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Venue description cannot exceed 2000 characters.");

            RuleFor(x => x.Address)
                .MaximumLength(500)
                .WithMessage("Venue address cannot exceed 500 characters.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .WithMessage("Venue capacity must be greater than 0.");
        }
    }
}
