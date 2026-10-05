using FluentValidation;

namespace EventFlow.Registration.Application.Features.Participants.Queries.GetParticipantById;

public sealed class GetParticipantByIdQueryValidator : AbstractValidator<GetParticipantByIdQuery>
{
    public GetParticipantByIdQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.ParticipantId)
            .NotEmpty()
            .WithMessage("Participant ID is required.");
    }
}
