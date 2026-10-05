using FluentValidation;

namespace EventFlow.Event.Application.Features.Sponsors.Commands.DeleteSponsor;

public sealed class DeleteSponsorCommandValidator : AbstractValidator<DeleteSponsorCommand>
{
    public DeleteSponsorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sponsor ID is required.");

        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
