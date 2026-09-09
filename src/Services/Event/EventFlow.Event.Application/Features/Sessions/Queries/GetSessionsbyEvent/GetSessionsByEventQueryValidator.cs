

using FluentValidation;

namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionsbyEvent
{
    public sealed class GetSessionsByEventQueryValidator
        : AbstractValidator<GetSessionsByEventQuery>
    {
        public GetSessionsByEventQueryValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
