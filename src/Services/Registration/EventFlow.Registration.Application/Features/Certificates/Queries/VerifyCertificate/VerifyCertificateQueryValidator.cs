using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Queries.VerifyCertificate;

public sealed class VerifyCertificateQueryValidator : AbstractValidator<VerifyCertificateQuery>
{
    public VerifyCertificateQueryValidator()
    {
        RuleFor(x => x.CertificateNumber)
            .NotEmpty()
            .WithMessage("Certificate number is required.");
    }
}
