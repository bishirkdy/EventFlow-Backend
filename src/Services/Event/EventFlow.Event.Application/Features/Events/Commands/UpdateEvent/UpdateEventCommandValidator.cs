
using FluentValidation;

namespace EventFlow.Event.Application.Features.Events.Commands.UpdateEvent
{
    public sealed class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(2000);

            RuleFor(x => x.EventType)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.SubType)
                .MaximumLength(100);

            RuleFor(x => x.StartDate)
                .NotEmpty();

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be greater than start date.");

            RuleFor(x => x.TimeZone)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
