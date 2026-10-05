using FluentValidation;

namespace EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakersByEvent;

public sealed class GetSpeakersByEventQueryValidator
    : AbstractValidator<GetSpeakersByEventQuery>
{
    public GetSpeakersByEventQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
