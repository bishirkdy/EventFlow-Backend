using FluentValidation;

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetEventPageById;

public sealed class GetEventPageByIdQueryValidator : AbstractValidator<GetEventPageByIdQuery>
{
    public GetEventPageByIdQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Page ID is required.");
    }
}
