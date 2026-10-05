using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.RevokeCertificate;

public sealed class RevokeCertificateCommandValidator : AbstractValidator<RevokeCertificateCommand>
{
    public RevokeCertificateCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(x => x.CertificateId)
            .NotEmpty()
            .WithMessage("Certificate ID is required.");
    }
}
