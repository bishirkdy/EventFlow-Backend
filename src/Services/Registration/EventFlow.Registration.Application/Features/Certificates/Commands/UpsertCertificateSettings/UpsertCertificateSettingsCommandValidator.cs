using EventFlow.Registration.Application.Common.Certificates;
using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.UpsertCertificateSettings;

public sealed class UpsertCertificateSettingsCommandValidator : AbstractValidator<UpsertCertificateSettingsCommand>
{
    private static readonly string[] AllowedModes = ["Classic", "ImageTemplate"];

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
            .WithMessage("Signatory designation must be 200 characters or fewer.");

        RuleFor(x => x.Request.ThemeColor)
            .Must(CertificateTemplateRules.IsHexColor)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.ThemeColor))
            .WithMessage("Theme color must be a hex color such as #2563EB.")
            .MaximumLength(9)
            .WithMessage("Theme color must be 9 characters or fewer.");

        RuleFor(x => x.Request.MinAttendancePercent)
            .InclusiveBetween(0, 100)
            .When(x => x.Request.MinAttendancePercent.HasValue)
            .WithMessage("Minimum attendance must be between 0 and 100.");

        RuleFor(x => x.Request.TemplateMode)
            .NotEmpty()
            .WithMessage("Template mode is required.")
            .Must(IsValidTemplateMode)
            .WithMessage("Template mode must be 'Classic' or 'ImageTemplate'.");

        RuleFor(x => x.Request.TemplateFields)
            .Must(fields => fields is null || fields.Count <= CertificateTemplateRules.MaxFields)
            .When(x => x.Request.TemplateFields is not null)
            .WithMessage($"A certificate template can contain at most {CertificateTemplateRules.MaxFields} fields.");

        RuleForEach(x => x.Request.TemplateFields)
            .ChildRules(field =>
            {
                field.RuleFor(f => f.Key)
                    .NotEmpty()
                    .WithMessage("Every template field requires a key.")
                    .Must(CertificateTemplateRules.IsAllowedKey)
                    .WithMessage(
                        "Template field keys must be one of: participantName, eventTitle, eventDates, " +
                        "certificateNumber, issueDate, signatoryName, signatoryTitle, title, subtitle, " +
                        "verificationQr, customText.");

                field.RuleFor(f => f.Text)
                    .MaximumLength(300)
                    .WithMessage("Template field text must be 300 characters or fewer.");

                field.RuleFor(f => f.XPercent)
                    .InclusiveBetween(0, 100)
                    .WithMessage("Template field X position must be between 0 and 100.");

                field.RuleFor(f => f.YPercent)
                    .InclusiveBetween(0, 100)
                    .WithMessage("Template field Y position must be between 0 and 100.");

                field.RuleFor(f => f.WidthPercent)
                    .InclusiveBetween(1, 100)
                    .WithMessage("Template field width must be between 1 and 100.");

                field.RuleFor(f => f.FontSizePt)
                    .InclusiveBetween(
                        CertificateTemplateRules.MinFontSizePt,
                        CertificateTemplateRules.MaxFontSizePt)
                    .WithMessage(
                        $"Template field font size must be between {CertificateTemplateRules.MinFontSizePt} " +
                        $"and {CertificateTemplateRules.MaxFontSizePt}.");

                field.RuleFor(f => f.FontColor)
                    .Must(CertificateTemplateRules.IsHexColor)
                    .WithMessage("Template field color must be a hex color such as #111827.");

                field.RuleFor(f => f.Align)
                    .Must(align => align is null || IsValidAlign(align))
                    .WithMessage("Template field alignment must be Left, Center or Right.");

                field.RuleFor(f => f.FontFamily)
                    .Must(family => family is null ||
                                   CertificateTemplateRules.AllowedFontFamilies.Contains(family.Trim()))
                    .WithMessage("Template field font family is not supported.");

                field.RuleFor(f => f.FontFamily)
                    .MaximumLength(50)
                    .WithMessage("Template field font family must be 50 characters or fewer.");
            });
    }

    private static bool IsValidTemplateMode(string? mode) =>
        AllowedModes.Contains(mode ?? "", StringComparer.OrdinalIgnoreCase);

    private static bool IsValidAlign(string? align) =>
        align is not null &&
        (align.Equals("Left", StringComparison.OrdinalIgnoreCase) ||
         align.Equals("Center", StringComparison.OrdinalIgnoreCase) ||
         align.Equals("Right", StringComparison.OrdinalIgnoreCase));
}
