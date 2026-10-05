using FluentValidation;

namespace EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakerById;

public sealed class GetSpeakerByIdQueryValidator : AbstractValidator<GetSpeakerByIdQuery>
{
    public GetSpeakerByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Speaker ID is required.");

        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
