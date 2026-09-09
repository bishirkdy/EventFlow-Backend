using FluentValidation;


namespace EventFlow.Event.Application.Features.Venues.Commands.DeleteVenue
{
    public sealed class DeleteVenueCommandValidator: AbstractValidator<DeleteVenueCommand>
    {
        public DeleteVenueCommandValidator()
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
