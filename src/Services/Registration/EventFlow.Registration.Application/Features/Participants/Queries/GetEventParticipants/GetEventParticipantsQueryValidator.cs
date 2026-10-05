using FluentValidation;

namespace EventFlow.Registration.Application.Features.Participants.Queries.GetEventParticipants;

public sealed class GetEventParticipantsQueryValidator : AbstractValidator<GetEventParticipantsQuery>
{
    public GetEventParticipantsQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid participant status.");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
    }
}
