using FluentValidation;

namespace EventFlow.Event.Application.Features.Events.Queries.GetPublicEvents;

public sealed class GetPublicEventsQueryValidator : AbstractValidator<GetPublicEventsQuery>
{
    public GetPublicEventsQueryValidator()
    {
        RuleFor(x => x.Take)
            .GreaterThan(0)
            .WithMessage("Take must be greater than 0.");
    }
}
