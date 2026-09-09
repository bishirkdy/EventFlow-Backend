
using FluentValidation;

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetEventPagesByEvent
{
    public sealed class GetEventPagesByEventQueryValidator: AbstractValidator<GetEventPagesByEventQuery>
    {
        public GetEventPagesByEventQueryValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
