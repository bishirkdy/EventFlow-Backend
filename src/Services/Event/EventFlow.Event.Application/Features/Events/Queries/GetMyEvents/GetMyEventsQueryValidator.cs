using FluentValidation;

namespace EventFlow.Event.Application.Features.Events.Queries.GetMyEvents
{
    // Query Validator
    internal sealed class GetMyEventsQueryValidator : AbstractValidator<GetMyEventsQuery>
    {
        public GetMyEventsQueryValidator()
        {
            // Validate user ID
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");
        }
    }
}
