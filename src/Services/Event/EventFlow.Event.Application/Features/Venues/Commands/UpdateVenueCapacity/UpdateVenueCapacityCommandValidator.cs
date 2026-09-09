

using FluentValidation;

namespace EventFlow.Event.Application.Features.Venues.Commands.UpdateVenueCapacity
{
    public sealed class UpdateVenueCapacityCommandValidator: AbstractValidator<UpdateVenueCapacityCommand>
    {
        public UpdateVenueCapacityCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Venue ID is required.");

            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .WithMessage("Venue capacity must be greater than 0.");
        }
    }
}
