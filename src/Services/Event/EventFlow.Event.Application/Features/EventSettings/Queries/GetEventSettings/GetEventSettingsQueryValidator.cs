using FluentValidation;


namespace EventFlow.Event.Application.Features.EventSettings.Queries.GetEventSettings
{
    // Query Validator
    internal sealed class GetEventSettingsQueryValidator
        : AbstractValidator<GetEventSettingsQuery>
    {
        public GetEventSettingsQueryValidator()
        {
            // Validate event ID
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
