using FluentValidation;

namespace EventFlow.Event.Application.Features.SessionSpeakers.Queries.GetSessionSpeakers;

public sealed class GetSessionSpeakersQueryValidator
    : AbstractValidator<GetSessionSpeakersQuery>
{
    public GetSessionSpeakersQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.SessionId)
            .NotEmpty()
            .WithMessage("Session ID is required.");
    }
}
