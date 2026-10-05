using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.UpsertCertificateSettings;

public sealed class UpsertCertificateSettingsCommandValidator : AbstractValidator<UpsertCertificateSettingsCommand>
{
    public UpsertCertificateSettingsCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.Request.Title)
            .NotEmpty()
            .WithMessage("Certificate title is required.")
            .MaximumLength(200)
            .WithMessage("Certificate title must be 200 characters or fewer.");

        RuleFor(x => x.Request.Subtitle)
            .MaximumLength(500)
            .WithMessage("Certificate subtitle must be 500 characters or fewer.");

        RuleFor(x => x.Request.SignatoryName)
            .MaximumLength(200)
            .WithMessage("Signatory name must be 200 characters or fewer.");

        RuleFor(x => x.Request.SignatoryTitle)
            .MaximumLength(200)
            .WithMessage("Signatory title must be 200 characters or fewer.");

        RuleFor(x => x.Request.ThemeColor)
            .MaximumLength(9)
            .WithMessage("Theme color must be 9 characters or fewer.");

        RuleFor(x => x.Request.MinAttendancePercent)
            .InclusiveBetween(0, 100)
            .When(x => x.Request.MinAttendancePercent.HasValue)
            .WithMessage("Minimum attendance must be between 0 and 100.");
    }
}
