using FluentValidation;

namespace EventFlow.Event.Application.Features.Events.Queries.GetMyEvents;

internal sealed class GetMyEventsQueryValidator : AbstractValidator<GetMyEventsQuery>
{
    public GetMyEventsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
