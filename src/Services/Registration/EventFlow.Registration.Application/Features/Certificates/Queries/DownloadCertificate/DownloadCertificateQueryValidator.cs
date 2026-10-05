using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.DownloadCertificate;

public sealed class DownloadCertificateQueryValidator : AbstractValidator<DownloadCertificateQuery>
{
    public DownloadCertificateQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.CertificateId)
            .NotEmpty()
            .WithMessage("Certificate ID is required.");
    }
}
