using EventFlow.Event.Application.Abstractions.Storage;
using FluentValidation;

namespace EventFlow.Event.Application.Features.Sponsors.Commands.CreateSponsor;

public sealed class CreateSponsorCommandValidator : AbstractValidator<CreateSponsorCommand>
{
    public CreateSponsorCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Sponsor name is required.")
            .MaximumLength(200)
            .WithMessage("Sponsor name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Sponsor description cannot exceed 2000 characters.");

        RuleFor(x => x.WebsiteUrl)
            .MaximumLength(500)
            .WithMessage("Sponsor website URL cannot exceed 500 characters.");

        RuleFor(x => x.SponsorLevel)
            .NotEmpty()
            .WithMessage("Sponsor level is required.")
            .MaximumLength(100)
            .WithMessage("Sponsor level cannot exceed 100 characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order cannot be negative.");

        RuleFor(x => x.Logo)
            .Must(logo => logo is null || logo.Length > 0)
            .WithMessage("Sponsor logo cannot be empty.");
    }
}
