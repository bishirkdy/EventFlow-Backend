using FluentValidation;

namespace EventFlow.Event.Application.Features.Events.Queries.GetEventIsPublished;

public sealed class GetEventIsPublishedQueryValidator : AbstractValidator<GetEventIsPublishedQuery>
{
    public GetEventIsPublishedQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
