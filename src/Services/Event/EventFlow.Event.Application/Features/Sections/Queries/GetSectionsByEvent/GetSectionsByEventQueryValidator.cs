using FluentValidation;


namespace EventFlow.Event.Application.Features.Sections.Queries.GetSectionsByEvent
{
    public sealed class GetSectionsByEventQueryValidator: AbstractValidator<GetSectionsByEventQuery>
    {
        public GetSectionsByEventQueryValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");
        }
    }
}
