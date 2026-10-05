using FluentValidation;

namespace EventFlow.Event.Application.Features.Events.Queries.GetEventById;

public sealed class GetEventByIdQueryValidator : AbstractValidator<GetEventByIdQuery>
{
    public GetEventByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
