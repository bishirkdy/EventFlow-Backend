using FluentValidation;

namespace EventFlow.Registration.Application.Features.Certificates.Commands.GenerateCertificates;

public sealed class GenerateCertificatesCommandValidator : AbstractValidator<GenerateCertificatesCommand>
{
    public GenerateCertificatesCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
