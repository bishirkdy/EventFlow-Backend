using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.UploadCertificateTemplateImage;

public sealed class UploadCertificateTemplateImageCommandValidator
    : AbstractValidator<UploadCertificateTemplateImageCommand>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/png",
        "image/jpeg",
        "image/jpg",
        "image/webp"
    ];

    public UploadCertificateTemplateImageCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("A certificate image file is required.");

        RuleFor(x => x.File!.Length)
            .GreaterThan(0)
            .WithMessage("The uploaded certificate image is empty.")
            .LessThanOrEqualTo(10 * 1024 * 1024)
            .WithMessage("The certificate image must be 10 MB or smaller.");

        RuleFor(x => x.File!.FileName)
            .NotEmpty()
            .WithMessage("The certificate image file name is required.");

        RuleFor(x => x.File!.ContentType)
            .Must(contentType => contentType is not null &&
                                 AllowedContentTypes.Contains(contentType.ToLowerInvariant()))
            .WithMessage("Only PNG, JPEG and WebP images are supported.");
    }
}
